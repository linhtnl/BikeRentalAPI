using AutoMapper;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/owners")]
    [ApiController]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class OwnerController : Controller
    {
        private readonly IOwnerService _ownerService;
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;
        public OwnerController(IOwnerService ownerService, IMapper mapper, IConfiguration configuration)
        {
            _ownerService = ownerService;
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Login([FromBody] OwnerLoginRequest request)
        {
            string token = await _ownerService.Login(request, _configuration);

            return await Task.Run(() => Ok(token));
        }

        [HttpPost("register")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Register([FromBody] OwnerRegisterRequest request)
        {
            string token = await _ownerService.Register(request, _configuration);

            return await Task.Run(() => Ok(token));
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] OwnerRatingViewModel model, int filterOption , int page = CommonConstants.DefaultPage)
        {
            return Ok(await _ownerService.GetAll(model,filterOption, page));
        }

        [HttpGet("areaid")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _ownerService.GetListOwnerByAreaId(id));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _ownerService.GetOwnerById(id));
        }
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            return Ok(await _ownerService.Delete(id));
        }

        [HttpPost("TestRead")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> Test([FromHeader] string token)
        {
            var result = new TokenService(_configuration).ReadJWTTokenToModel(token);

            return await Task.Run(() => Ok(result));
        }

        [HttpPost("TestGenerate")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> TestGenerate(OwnerViewModel ownerInfo)
        {
            var result = new TokenService(_configuration).GenerateOwnerJWTWebToken(ownerInfo);

            return await Task.Run(() => Ok(result));
        }
    }
}

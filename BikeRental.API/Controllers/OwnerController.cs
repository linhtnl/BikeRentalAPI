using AutoMapper;
using BikeRental.API.Models.Request;
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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public OwnerController(IOwnerService ownerService, IMapper mapper, IConfiguration configuration)
        {
            _ownerService = ownerService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.GoogleId); // get user by request's guid
            OwnerViewModel result = await _ownerService.GetByMail(userRecord.Email);

            if (result != null) // if email existed in local database
            {
                FirebaseToken token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.AccessToken); // re-check access token with firebase
                object email;
                token.Claims.TryGetValue("email", out email); // get email from the above re-check step, then check the email whether it's matched the request email
                if (userRecord.Email.Equals(email))
                {
                    string verifyRequestToken = new TokenService(_configuration).GenerateOwnerJWTWebToken(result);

                    return await Task.Run(() => Ok(verifyRequestToken)); // return if everything is done
                }
                return await Task.Run(() => StatusCode(ResponseStatusConstants.FORBIDDEN)); // return if this email's not existed yet in database - FE foward to sign up page
            }
            var claim = new Dictionary<string, object> { { "email", userRecord.Email } };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(request.GoogleId, claim);

            return await Task.Run(() => StatusCode(ResponseStatusConstants.CREATED));
        }

        [HttpPost("register")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            FirebaseToken token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.AccessToken); // get firebase token via request's access token
            object email;
            token.Claims.TryGetValue("email", out email); // get email from token above

            if (request.Owner.Mail.Equals(email)) // re-check if the email in request and email in the token above is match
            {
                Owner owner = _mapper.Map<Owner>(request.Owner);
                OwnerViewModel ownerResult = await _ownerService.CreateNew(owner);

                if (ownerResult != null)
                {
                    string verifyRequestToken = new TokenService(_configuration).GenerateOwnerJWTWebToken(ownerResult);

                    return await Task.Run(() => Ok(verifyRequestToken));
                }
                return await Task.Run(() => StatusCode(ResponseStatusConstants.FORBIDDEN));
            }
            return await Task.Run(() => StatusCode(ResponseStatusConstants.FORBIDDEN));
        }
        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] OwnerRatingViewModel model, int filterOption , int page = CommonConstants.DefaultPage)
        {
            return Ok(await _ownerService.GetAll(model,filterOption, page));
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

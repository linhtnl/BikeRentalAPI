using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;

using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/bikes")]
    [ApiController]
    [ApiVersion("1")]
    public class BikeController : ControllerBase
    {
        private readonly IBikeService _bikeService;
        private readonly IBikeUtilService _bikeUtilService;
        public BikeController(IBikeService bikeService, IBikeUtilService bikeUtilService)
        {
            _bikeService = bikeService;
            _bikeUtilService = bikeUtilService;
        }
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _bikeService.GetBikeById(id));
        }

        [HttpGet]
        [MapToApiVersion("1")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] BikeViewModel model,int size, int page = CommonConstants.DefaultPage)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count>0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bikeService.GetAll(model, size, page, token));
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]BikeCreateRequest request)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bikeService.Create(request, token));
        }
        [HttpPut]
        [MapToApiVersion("1")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] BikeUpdateRequest request)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bikeService.Update(request,token));
        }
        [HttpDelete]
        [MapToApiVersion("1")]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bikeService.Delete(id,token));
        }
        
    }
}

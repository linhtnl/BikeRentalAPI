using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/bikes")]
    [ApiController]
    [ApiVersion("1")]
    public class BikeController : ControllerBase
    {
        private readonly IBikeService _bikeService;

        public BikeController(IBikeService bikeService)
        {
            _bikeService = bikeService;
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _bikeService.GetBikeById(id));
        }

        [Authorize]
        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] BikeViewModel model,int size, int page = CommonConstants.DefaultPage)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count>0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bikeService.GetAll(model, size, page, token));
        }

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create([FromBody]BikeCreateRequest request)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bikeService.Create(request, token));
        }

        [Authorize]
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update([FromBody] BikeUpdateRequest request)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bikeService.Update(request,token));
        }

        [Authorize]
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bikeService.Delete(id,token));
        }

        [HttpGet("suitableBike/{ownerId}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetSuitableBikeByOwnerId(Guid ownerId)
        {
            return Ok(await _bikeService.GetSuitableBikeByOwnerId(ownerId));
        }
    }
}

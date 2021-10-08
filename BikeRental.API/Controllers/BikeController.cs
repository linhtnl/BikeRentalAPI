using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;

using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] BikeViewModel model, int page = CommonConstants.DefaultPage)
        {
            return Ok(await _bikeService.GetAll(model, page));
        }

        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create([FromBody]BikeCreateRequest request)
        {
            return Ok(await _bikeService.Create(request));
        }
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update([FromBody] BikeUpdateRequest request)
        {
            return Ok(await _bikeService.Update(request));
        }
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            return Ok(await _bikeService.Delete(id));
        }
    }
}

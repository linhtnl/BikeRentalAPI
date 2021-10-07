using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/pricelists")]
    [ApiController]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class PriceListController : ControllerBase
    {
        private readonly IPriceListService _priceListService;

        public PriceListController(IPriceListService priceListService)
        {
            _priceListService = priceListService;
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAll([FromQuery] PriceListViewModel model)
        {
            return Ok(await _priceListService.GetAll(model));
        }
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create(PricelistCreateRequest request)
        {
            return Ok(await _priceListService.Create(request));
        }

        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(Guid areaId, Guid cateId, decimal? price)
        {
            return Ok(await _priceListService.Update(areaId,cateId,price));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetByAreaId(Guid id)
        {
            return Ok(await _priceListService.GetListByAreaId(id));
        }
    }
}

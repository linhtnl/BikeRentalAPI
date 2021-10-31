using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _priceListService.GetAll());
        }

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create(PricelistCreateRequest request)
        {
            return Ok(await _priceListService.Create(request));
        }

        [Authorize]
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(Guid areaId, Guid motorTypeId, decimal? price)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            var result = await _priceListService.Update(areaId, motorTypeId, price, token);

            return await Task.Run(() => Ok(result));
        }

        [HttpGet("suitablePriceList")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetPriceByAreaIdAndTypeId(Guid areaId, Guid typeId)
        {
            return Ok(await _priceListService.GetPriceByAreaIdAndTypeId(areaId, typeId));
        }

        [HttpGet("listByArea")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetByAreaId(Guid areaId)
        {
            return Ok(await _priceListService.GetPriceByArea(areaId));
        }
    }
}

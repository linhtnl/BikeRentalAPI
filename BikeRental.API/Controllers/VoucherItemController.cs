using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/voucherItems")]
    [ApiController]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class VoucherItemController : Controller
    {
        private readonly IVoucherItemService _voucherItemService;

        public VoucherItemController(IVoucherItemService voucherItemService)
        {
            _voucherItemService = voucherItemService;
        }

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> CreateNew([FromBody] VoucherItemCreateRequest voucherItemRequest)
        {
            var voucherItem = await _voucherItemService.CreateNew(voucherItemRequest);

            return await Task.Run(() => Ok(voucherItem));
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public List<VoucherItemViewModel> GetAll()
        {
            return _voucherItemService.GetAll();
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public VoucherItemViewModel GetById(Guid id)
        {
            return _voucherItemService.GetById(id);
        }

        [HttpGet("customerId/{customerId}")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            var result = await _voucherItemService.GetByCustomerId(customerId);
            return await Task.Run(() => Ok(result));
        }

        [HttpGet("voucherId/{voucherId}")]
        [MapToApiVersion("2")]
        public List<VoucherItemViewModel> GetByVoucherId(Guid voucherId)
        {
            return _voucherItemService.GetByVoucherId(voucherId);
        }
    }
}

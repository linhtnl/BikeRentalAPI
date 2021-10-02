using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v1.0/voucherItems")]
    [ApiController]
    public class VoucherItemController : Controller
    {
        private readonly IVoucherItemService _voucherItemService;
        public VoucherItemController(IVoucherItemService voucherItemService)
        {
            _voucherItemService = voucherItemService;
        }

        [HttpPost("create")]
        public async Task<bool> CreateNew(VoucherItemViewModel voucherItemRequest)
        {
            return await _voucherItemService.CreateNew(voucherItemRequest);
        }

        [HttpGet]
        public List<VoucherItemViewModel> GetAll()
        {
            return _voucherItemService.GetAll();
        }

        [HttpGet("id/{id}")]
        public VoucherItemViewModel GetById(Guid id)
        {
            return _voucherItemService.GetById(id);
        }

        [HttpGet("customerId/{customerId}")]
        public List<VoucherItemViewModel> GetByCustomerId(Guid customerId)
        {
            return _voucherItemService.GetByCustomerId(customerId);
        }

        [HttpGet("voucherId/{voucherId}")]
        public List<VoucherItemViewModel> GetByVoucherId(Guid voucherId)
        {
            return _voucherItemService.GetByVoucherId(voucherId);
        }
    }
}

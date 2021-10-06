using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/vouchers")]
    [ApiController]
    [ApiVersion("1")]
    public class VoucherController : Controller
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> CreateNew([FromBody] VoucherCreateRequest voucherRequest)
        {
            Voucher voucher = await _voucherService.CreateNew(voucherRequest);

            return voucher != null
                ? await Task.Run(() => Ok(voucher))
                : await Task.Run(() => StatusCode(ResponseStatusConstants.FORBIDDEN));
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public List<VoucherViewModel> GetAll()
        {
            return _voucherService.GetAll();
        }

        [HttpGet("id/{id}")]
        [MapToApiVersion("1")]
        public VoucherViewModel GetById(Guid id)
        {
            return _voucherService.GetById(id);
        }

        [HttpGet("campaignId/{campaignId}")]
        [MapToApiVersion("1")]
        public List<VoucherViewModel> GetByCampaignId(Guid campaignId)
        {
            return _voucherService.GetByCampaignId(campaignId);
        }

        [HttpGet("inRangeDate/start")]
        [MapToApiVersion("1")]
        public List<VoucherViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _voucherService.GetStartInRangeDate(startDate, endDate);
        }

        [HttpGet("inRangeDate/end")]
        [MapToApiVersion("1")]
        public List<VoucherViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _voucherService.GetEndInRangeDate(startDate, endDate);
        }

        [HttpGet("inRangeDiscountPercent")]
        [MapToApiVersion("1")]
        public List<VoucherViewModel> GetInDiscountPercentRange(int startNum, int endNum)
        {
            return _voucherService.GetInDiscountPercentRange(startNum, endNum);
        }

        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            return await Task.Run(() => Ok(_voucherService.DeleteVoucher(id)));
        }

        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(Guid id, VoucherUpdateRequest voucherRequest)
        {
            return await Task.Run(() => Ok(_voucherService.UpdateVoucher(id, voucherRequest)));
        }
    }
}

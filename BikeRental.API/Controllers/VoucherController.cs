using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/vouchers")]
    [ApiController]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class VoucherController : Controller
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> CreateNew([FromBody] VoucherCreateRequest voucherRequest)
        {
            Voucher voucher = await _voucherService.CreateNew(voucherRequest);

            return await Task.Run(() => Ok(voucher));
        }

        [Authorize]
        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAll()
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            var result = await _voucherService.GetAll(token);

            return await Task.Run(() => Ok(result));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _voucherService.GetById(id);

            return await Task.Run(() => Ok(result));
        }

        [HttpGet("campaignId/{campaignId}")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetByCampaignId(Guid campaignId)
        {
            var result = await _voucherService.GetByCampaignId(campaignId);

            return await Task.Run(() => Ok(result));
        }

        [HttpGet("inRangeDate/start")]
        [MapToApiVersion("2")]
        public List<VoucherViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _voucherService.GetStartInRangeDate(startDate, endDate);
        }

        [HttpGet("inRangeDate/end")]
        [MapToApiVersion("2")]
        public List<VoucherViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _voucherService.GetEndInRangeDate(startDate, endDate);
        }

        [HttpGet("inRangeDiscountPercent")]
        [MapToApiVersion("2")]
        public List<VoucherViewModel> GetInDiscountPercentRange(int startNum, int endNum)
        {
            return _voucherService.GetInDiscountPercentRange(startNum, endNum);
        }

        [Authorize]
        [HttpDelete]
        [MapToApiVersion("2")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            return await Task.Run(() => Ok(_voucherService.DeleteVoucher(id)));
        }

        [Authorize]
        [HttpPut]
        [MapToApiVersion("2")]
        public async Task<IActionResult> Update(Guid id, VoucherUpdateRequest voucherRequest)
        {
            return await Task.Run(() => Ok(_voucherService.UpdateVoucher(id, voucherRequest)));
        }

        [HttpGet("GetDiscountedPrice")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetDiscountedPrice(decimal originalPrice, Guid voucherCode)
        {
            var finalPrice = await _voucherService.GetDiscountedPrice(originalPrice, voucherCode);

            return await Task.Run(() => Ok(finalPrice));
        }

        [Authorize]
        [HttpGet("GetByAreaId")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetByAreaId(Guid areaId)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            var result = await _voucherService.GetByAreaId(areaId, token);

            return await Task.Run(() => Ok(result));
        }
    }
}

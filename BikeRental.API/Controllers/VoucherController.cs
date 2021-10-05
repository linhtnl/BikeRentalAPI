using BikeRental.Business.Services;
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
        public async Task<bool> CreateNew([FromBody] VoucherViewModel voucherRequest)
        {
            return await _voucherService.CreateNew(voucherRequest);
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
    }
}

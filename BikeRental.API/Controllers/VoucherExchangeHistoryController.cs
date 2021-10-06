using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/voucherExchangeHistories")]
    public class VoucherExchangeHistoryController : Controller
    {
        private readonly IVoucherExchangeHistoryService _voucherExchangeHistoryService;
        public VoucherExchangeHistoryController(IVoucherExchangeHistoryService voucherExchangeHistoryService)
        {
            _voucherExchangeHistoryService = voucherExchangeHistoryService;
        }

        //[HttpPost]
        //[MapToApiVersion("1")]
        //public async Task<IActionResult> CreateNew(VoucherExchangeHistoryCreateRequest request)
        //{
        //    //var resultVoucherExchangeHistory = await _voucherExchangeHistoryService.CreateNew(request);

        //    //return resultVoucherExchangeHistory != null
        //    //    ? await Task.Run(() => Ok(resultVoucherExchangeHistory))
        //    //    : await Task.Run(() => StatusCode(ResponseStatusConstants.FORBIDDEN));
        //}

        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(Guid id, VoucherExchangeHistoryUpdateRequest request)
        {
            return Ok(await _voucherExchangeHistoryService.UpdateVoucherExchangeHistory(id, request));
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _voucherExchangeHistoryService.GetById(id));
        }
    }
}

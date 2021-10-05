﻿using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/voucherItems")]
    [ApiController]
    [ApiVersion("1")]
    public class VoucherItemController : Controller
    {
        private readonly IVoucherItemService _voucherItemService;
        public VoucherItemController(IVoucherItemService voucherItemService)
        {
            _voucherItemService = voucherItemService;
        }

        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<bool> CreateNew([FromBody]VoucherItemViewModel voucherItemRequest)
        {
            return await _voucherItemService.CreateNew(voucherItemRequest);
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public List<VoucherItemViewModel> GetAll()
        {
            return _voucherItemService.GetAll();
        }

        [HttpGet("id/{id}")]
        [MapToApiVersion("1")]
        public VoucherItemViewModel GetById(Guid id)
        {
            return _voucherItemService.GetById(id);
        }

        [HttpGet("customerId/{customerId}")]
        [MapToApiVersion("1")]
        public List<VoucherItemViewModel> GetByCustomerId(Guid customerId)
        {
            return _voucherItemService.GetByCustomerId(customerId);
        }

        [HttpGet("voucherId/{voucherId}")]
        [MapToApiVersion("1")]
        public List<VoucherItemViewModel> GetByVoucherId(Guid voucherId)
        {
            return _voucherItemService.GetByVoucherId(voucherId);
        }
    }
}
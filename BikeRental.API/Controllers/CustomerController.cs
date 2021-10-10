﻿using AutoMapper;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/customers")]
    [ApiController]
    [ApiVersion("1")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IConfiguration _configuration;
        public CustomerController(ICustomerService customerService, IConfiguration configuration)
        {
            _customerService = customerService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginRequest request)
        {
            string token = await _customerService.Login(request.PhoneNumber, _configuration);

            return await Task.Run(() => Ok(token));
        }

        [HttpPost("register")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Register([FromBody] CustomerCreateRequest request)
        {
            string token = await _customerService.Register(request, _configuration);

            return await Task.Run(() => Ok(token));
        }

        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(CustomerUpdateRequest request)
        {
            var updatedCustomer = await _customerService.UpdateCustomer(request);

            return await Task.Run(() => Ok(updatedCustomer));
        }

        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _customerService.DeleteCustomer(id);

            return await Task.Run(() => Ok(isDeleted));
        }
    }
}

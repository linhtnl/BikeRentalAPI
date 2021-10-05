using BikeRental.API.Models.Request;
using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /*[HttpGet("id/{id}")]
        [MapToApiVersion("1")]
        public CustomerViewModel GetCustomerById(Guid id)
        {
            return _customerService.GetCustomerById(id);
        }

        [HttpGet("phone/{phone}")]
        [MapToApiVersion("1")]
        public CustomerViewModel GetCustomerByPhone(string phone)
        {
            return _customerService.GetCustomerByPhone(phone);
        }*/
    }
}

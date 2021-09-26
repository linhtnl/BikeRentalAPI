using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("id/{id}")]
        public CustomerViewModel GetCustomerById(Guid id)
        {
            return _customerService.GetCustomerById(id);
        }

        [HttpGet("phone/{phone}")]
        public CustomerViewModel GetCustomerByPhone(string phone)
        {
            return _customerService.GetCustomerByPhone(phone);
        }
    }
}

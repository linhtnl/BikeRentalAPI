using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/admins")]
    [ApiController]
    [ApiVersion("2")]   
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IConfiguration _configuration;

        public AdminController(IAdminService adminService, IConfiguration configuration)
        {
            _adminService = adminService;
            _configuration = configuration;
        }

        [HttpGet("customers")]
        [MapToApiVersion("2")]
        public List<CustomerViewModel> GetAll()
        {
            return _adminService.GetAllCustomer();
        }

        [HttpGet("customers/id/{id}")]
        [MapToApiVersion("2")]
        public CustomerViewModel GetCustomerById(Guid id)
        {
            return _adminService.GetCustomerById(id);
        }

        [HttpGet("customers/phone/{phone}")]
        [MapToApiVersion("2")]
        public CustomerViewModel GetCustomerByPhone(string phone)
        {
            return _adminService.GetCustomerByPhone(phone);
        }

        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            string token = await _adminService.Login(request, _configuration);

            return await Task.Run(() => Ok(token));
        }
    }
}

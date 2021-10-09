using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;

        }

        //GET All Admin  
        /* [HttpGet("GetAllAdmins")]
         public Object GetAllAdmins()
         {
             var data = _adminService.GetAllAdmins();
             var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                 new JsonSerializerSettings()
                 {
                     ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                 }
             );
             return json;
         }*/

        // Add Admin
        /*[HttpPost("AddAdmins")]
        public async Task<Object> AddAdmin([FromBody] AdminRequest admin)
        {
            BikeRental.Data.Models.Admin newAdmin = new BikeRental.Data.Models.Admin
            {
                Id = admin.Id,
                UserName = admin.Username,
                Password = admin.Password
            };
            try
            {
                await _adminService.AddAdmin(newAdmin);
                return true;
            }
            catch
            {
                return false;
            }
        }*/

        //List Customer
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
    }
}

using BikeRental.API.Models;
using BikeRental.API.Models.Request;

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
    [Route("api/admin")]
    [ApiController]
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
        public List<CustomerViewModel> GetAll()
        {
            return _adminService.GetAllCustomer();
        }
    }
}

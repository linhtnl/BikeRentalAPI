using BikeRental.API.Models;
using BikeRental.API.Models.Request;
using BikeRental.Business.Service;
using BikeRental.Data.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService AdminService)
        {
            _adminService = AdminService;

        }

        //GET All Admin  
        [HttpGet("GetAllAdmins")]
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
        }

        // Add Admin
        [HttpPost("AddAdmins")]
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
        }
    }
}

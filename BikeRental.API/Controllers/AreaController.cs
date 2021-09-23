using BikeRental.Business.Service;
using BikeRental.Data.Interface;
using BikeRental.Data.Models;
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
    public class AreaController : Controller
    {
        private readonly AreaService _areaService;
        public AreaController(AreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet("GetAllArea")]
        //Get ALL Area
        public Object getAllArea()
        {
            var data = _areaService.getAll();
            var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            );
            return json;
        }

        [HttpGet("GetAreaById")]
        public Object getAreaById(string id)
        {
            var data = _areaService.getAreaById(id);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            );
            return json;
        }

        [HttpGet("GetAreaByName")]
        public Object getAreaByName(string name)
        {
            var data = _areaService.getAreaByName(name);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });
            return json;
        }

        [HttpGet("GetAreaByPostalCode")]
        public Object getAreaByPostalCode(int postalCode)
        {
            var data = _areaService.getAreaByPostalCode(postalCode);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });
            return json;
        }
    }
}

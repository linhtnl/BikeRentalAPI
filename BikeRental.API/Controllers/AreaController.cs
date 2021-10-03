
using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v1.0/areas")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;
        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        public List<AreaViewModel> GetAllArea()
        {
            return _areaService.GetAll();
        }

        [HttpGet("id/{id}")]
        public AreaViewModel GetArea(Guid id)
        {
            return _areaService.GetById(id);
        }

        [HttpGet("name/{name}")]
        public AreaViewModel GetAreaByName(string name)
        {
            return _areaService.GetByName(name);
        }

        [HttpGet("postalCode/{postalCode}")]
        public AreaViewModel GetAreaByPostalCode(int postalCode)
        {
            return _areaService.GetAreaByPostalCode(postalCode);
        }
        [HttpPut] // this method must be implement checking verifyRequestToken in the header before action (login methods havent been implemented yet)
        public async Task<Area> Update(Guid id , int postalCode, string name)
        {
                return await _areaService.Update(id, postalCode, name);
        }
        [HttpPost]
        public async Task<Area> Create(AreaCreateModel model)
        {
            return await _areaService.Create(model);
        }
    }
}

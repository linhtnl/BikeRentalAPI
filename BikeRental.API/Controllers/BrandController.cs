using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v1.0/brands")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public List<BrandViewModel> GetAll()
        {
            return _brandService.GetAll();
        }
        
        [HttpGet("id/{id}")]
        public BrandViewModel GetById(Guid id)
        {
            return _brandService.GetBrandById(id);
        }
        [HttpPut]
        public async Task<Brand> Update(Guid id, string name)
        {
            return await _brandService.Update(id, name);
        }
        [HttpPost]
        public async Task<Brand> Create(string name)
        {
            return await _brandService.Create(name);
        }
    }
}

using BikeRental.Business.Constants;
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
    [Route("api/v{version:apiVersion}/brands")]
    [ApiController]
    [ApiVersion("1")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] BrandViewModel model, int page = CommonConstants.DefaultPage)
        {
            return Ok(await _brandService.GetAll(model, page));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _brandService.GetBrandById(id));
        }
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(Guid id, string name)
        {
            return Ok(await _brandService.Update(id, name));
        }
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create(string name)
        {
            return Ok(await _brandService.Create(name));
        }
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _brandService.Delete(id));
        }
    }
}

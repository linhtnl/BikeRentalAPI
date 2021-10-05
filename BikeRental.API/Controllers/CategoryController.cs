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
    [Route("api/v{version:apiVersion}/categories")]
    [ApiController]
    [ApiVersion("1")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] CategoryViewModel model, int page = CommonConstants.DefaultPage)
        {
            return Ok(await _categoryService.GetAll(model, page));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetCateById(Guid id)
        {
            return Ok(await _categoryService.GetCateById(id));
        }
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<Category> Update(Guid id, string name, int type)
        {
            return await _categoryService.Update(id, name, type);
        }
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<Category> Create(CategoryCreateModel model)
        {
            return await _categoryService.Create(model);
        }
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<bool> Delete(Guid id)
        {
            return await _categoryService.Delete(id);
        }
    }
}

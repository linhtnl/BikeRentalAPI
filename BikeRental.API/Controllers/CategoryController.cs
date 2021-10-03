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
    [Route("api/v1.0/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public List<CategoryViewModel> GetAll()
        {
            return _categoryService.GetAllCate();
        }

        [HttpGet("id/{id}")]
        public CategoryViewModel GetCateById(Guid id)
        {
            return _categoryService.GetCateById(id);
        }

        [HttpGet("type/{type}")]
        public List<CategoryViewModel> GetCateByType(int type)
        {
            return _categoryService.GetCateByType(type);
        }
        [HttpPut]
        public async Task<Category> Update(Guid id, string name, int type)
        {
            return await _categoryService.Update(id, name, type);
        }
        [HttpPost]
        public async Task<Category> Create(CategoryCreateModel model)
        {
            return await _categoryService.Create(model);
        }
    }
}

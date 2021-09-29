using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/categories")]
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
        public CategoryViewModel GetCateByType(int type)
        {
            return _categoryService.GetCateByType(type);
        }
    }
}

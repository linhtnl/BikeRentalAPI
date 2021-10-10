using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.Utilities;
using BikeRental.Data.Enums;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        Task<List<BrandViewModel>> GetAll(BrandViewModel model);
        Task<BrandViewModel> GetBrandById(Guid? id);

        Task<Brand> Update(Guid id, string name);

        Task<Brand> Create(string name);
        Task<bool> Delete(Guid id);
    }
    public class BrandService : BaseService<Brand>, IBrandService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly ICategoryService _categoryService;

        public BrandService(IUnitOfWork unitOfWork, ICategoryService categoryService, IBrandRepository repository , IMapper mapper) : base(unitOfWork, repository)
        {
            _categoryService = categoryService;
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Brand> Create(string name)
        {
            var brand = new Brand();
            brand.Name = name;
            try
            {
                await CreateAsync(brand);
            }
            catch
            {
                throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "Invalid data");
            }
            
            return brand;
        }

        public async Task<bool> Delete(Guid id)
        {
            bool result = false;
            var brand = Get(b => b.Id.Equals(id)).FirstOrDefault();
            if (brand == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var cates = await _categoryService.GetCateByBrandId(id);
            foreach(var cate in cates)
            {
                if(cate.Status!=(int)CategoryStatus.Unavailable) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Can not Delete");
            }
            brand.Status = (int)BrandStatus.Unavailable;
            try
            {
                await UpdateAsync(brand);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
        

        //not yet
        public async Task<List<BrandViewModel>> GetAll(BrandViewModel model)
        {
            var brands = Get(b => b.Status ==(int)BrandStatus.Available).ProjectTo<BrandViewModel>(_mapper);
            var listBrand = brands.ToList();
            for (int i = 0; i < listBrand.Count; i++)
            {
                var listCateTemp = new List<Category>();
                var listCate = await _categoryService.GetCateByBrandId(Guid.Parse(listBrand[i].id.ToString()));
                foreach(var cate in listCate)
                {
                    if (cate.Status == (int)CategoryStatus.Available)
                    {
                        listCateTemp.Add(cate);
                    }
                }
                listBrand[i].ListCategory = _mapper.CreateMapper().Map<List<CategoryCustomViewModel>>(listCateTemp);
            }
            brands = listBrand.AsQueryable().OrderByDescending(b => b.Name);
            var result = brands.DynamicFilter(model).ToList();
            if(result.Count()==0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            return result;
            
        }

        public async Task<BrandViewModel> GetBrandById(Guid? id)
        {
            var brand = await Get(b => b.Id.Equals(id)).ProjectTo<BrandViewModel>(_mapper).FirstOrDefaultAsync();
            if(brand == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var listCateTemp = new List<Category>();
            var listCate = await _categoryService.GetCateByBrandId(Guid.Parse(brand.id.ToString()));
            foreach (var cate in listCate)
            {
                if (cate.Status == (int)CategoryStatus.Available)
                {
                    listCateTemp.Add(cate);
                }
            }
            brand.ListCategory = _mapper.CreateMapper().Map<List<CategoryCustomViewModel>>(listCateTemp);
            return brand;
        }

        public async Task<Brand> Update(Guid id, string name)
        {
            Brand brand = Get(b => b.Id.Equals(id)).FirstOrDefault();
            brand.Name = name;
            await UpdateAsync(brand);
            return brand;
        }
    }
}

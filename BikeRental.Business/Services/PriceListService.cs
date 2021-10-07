using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Utilities;
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
    public interface IPriceListService : IBaseService<PriceList>
    {
        Task<List<PriceListViewModel>> GetAll(PriceListViewModel model);
        Task<PriceList> Create(PricelistCreateRequest request);
        Task<PriceList> Update(Guid areaId, Guid cateId, decimal? price);
        Task<List<PriceListViewModel>> GetListByAreaId(Guid areaId);
    }
    public class PriceListService : BaseService<PriceList>, IPriceListService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        public PriceListService(IUnitOfWork unitOfWork, IPriceListRepository repository, ICategoryService categoryService, IBrandService brandService, IMapper mapper) : base(unitOfWork, repository)
        {
            _categoryService = categoryService;
            _brandService = brandService;
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<PriceList> Create(PricelistCreateRequest request)
        {
            var priceList = _mapper.CreateMapper().Map<PriceList>(request);
            await CreateAsync(priceList);
            return priceList;
        }

        public async Task<List<PriceListViewModel>> GetAll(PriceListViewModel model)
        {
            var priceList = Get().ProjectTo<PriceListViewModel>(_mapper);
            List<PriceListViewModel> list = priceList.ToList();
            if (list.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            for(int i = 0; i < list.Count; i++)
            {
                var cate = await _categoryService.GetCateById(list[i].CategoryId);
                list[i].CateName = cate.Name;
                var brand = await _brandService.GetBrandById(cate.BrandId);
                list[i].BrandName = brand.Name;
            }
            priceList = list.AsQueryable().OrderByDescending(p => p.Price);
            var result = priceList.DynamicFilter(model);
            return result.ToList();
        }

        public async Task<List<PriceListViewModel>> GetListByAreaId(Guid areaId)
        {
            var priceList = await Get(a => a.AreaId.Equals(areaId)).ProjectTo<PriceListViewModel>(_mapper).ToListAsync();
            return priceList;
        }

        public async Task<PriceList> Update(Guid areaId, Guid cateId, decimal? price)
        {
            var priceList = await Get(pl => pl.AreaId.Equals(areaId) && pl.CategoryId.Equals(cateId)).FirstOrDefaultAsync();
            priceList.Price = price;
            await UpdateAsync(priceList);
            return priceList;
        }
    }
}

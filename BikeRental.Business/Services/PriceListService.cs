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
        Task<List<PriceListViewModel>> GetAll();
        Task<PriceList> Create(PricelistCreateRequest request);
        Task<PriceList> Update(Guid areaId, Guid cateId, decimal? price);
        Task<List<PriceListViewModel>> GetListByAreaIdAndTypeId(Guid areaId,Guid typeId);
        Task<decimal> GetPriceByAreaIdAndCategoryId(Guid areaId, Guid categoryId);
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

        public async Task<List<PriceListViewModel>> GetAll()
        {
            var priceList = await Get().ProjectTo<PriceListViewModel>(_mapper).ToListAsync();
            if (priceList.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            return priceList;
        }

        public async Task<List<PriceListViewModel>> GetListByAreaIdAndTypeId(Guid areaId, Guid typeId)
        {
            var priceList = await Get(a => a.AreaId.Equals(areaId )&& a.MotorTypeId.Equals(typeId)).ProjectTo<PriceListViewModel>(_mapper).ToListAsync();
            if (priceList.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            return priceList;
        }

        public async Task<decimal> GetPriceByAreaIdAndCategoryId(Guid areaId, Guid categoryId)
        {
            /*var priceList = await Get()
                .Where(priceTemp => (priceTemp.CategoryId.Equals(categoryId) && priceTemp.AreaId.Equals(areaId)))
                .FirstOrDefaultAsync();

            if (priceList == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Price of this area and category is not found.");

            return await Task.Run(() => priceList.Price.Value);*/
            return 0;
        }

        public async Task<PriceList> Update(Guid areaId, Guid cateId, decimal? price)
        {
            /*var priceList = await Get(pl => pl.AreaId.Equals(areaId) && pl.CategoryId.Equals(cateId)).FirstOrDefaultAsync();
            priceList.Price = price;
            await UpdateAsync(priceList);
            return priceList;*/
            return null;
        }
    }
}

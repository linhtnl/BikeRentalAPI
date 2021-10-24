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
        Task<decimal> GetPriceByAreaIdAndTypeId(Guid areaId,Guid typeId);
        Task<List<PriceListByAreaViewModel>> GetPriceByArea(Guid areaId);
    }
    public class PriceListService : BaseService<PriceList>, IPriceListService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly IMotorTypeService _motorTypeService;
        public PriceListService(IUnitOfWork unitOfWork, IPriceListRepository repository, IMotorTypeService motorTypeService, IMapper mapper) : base(unitOfWork, repository)
        {
            _motorTypeService = motorTypeService;
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

        public async Task<decimal> GetPriceByAreaIdAndTypeId(Guid areaId, Guid typeId)
        {
            PriceList priceList = await Get()
                .Where(priceTemp => (priceTemp.AreaId.Equals(areaId) && priceTemp.MotorTypeId.Equals(typeId)))
                .FirstOrDefaultAsync();
            if(priceList == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            return await Task.Run(() => priceList.Price.Value);
        }

        public async Task<PriceList> Update(Guid areaId, Guid cateId, decimal? price)
        {
            /*var priceList = await Get(pl => pl.AreaId.Equals(areaId) && pl.CategoryId.Equals(cateId)).FirstOrDefaultAsync();
            priceList.Price = price;
            await UpdateAsync(priceList);
            return priceList;*/
            return null;
        }

        public async Task<List<PriceListByAreaViewModel>> GetPriceByArea(Guid areaId)
        {
            var priceList = Get(p => p.AreaId.Equals(areaId)).ProjectTo<PriceListByAreaViewModel>(_mapper);
            var list = priceList.ToList();
            if(list.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            foreach (var price in list)
            {
                var type = await _motorTypeService.GetById(price.MotorTypeId);
                price.TypeName = type.Name;
            }
            return list;          
        }
    }
}

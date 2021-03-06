using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Utilities;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        Task<PriceList> Update(Guid areaId, Guid cateId, decimal? price, string token);
        Task<decimal> GetPriceByAreaIdAndTypeId(Guid areaId,Guid typeId);
        Task<List<PriceListByAreaViewModel>> GetPriceByArea(Guid areaId);
    }
    public class PriceListService : BaseService<PriceList>, IPriceListService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;

        private readonly IMotorTypeService _motorTypeService;
        public PriceListService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, 
            IPriceListRepository repository, 
            IMotorTypeService motorTypeService) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _motorTypeService = motorTypeService;
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

        public async Task<PriceList> Update(Guid areaId, Guid motorTypeId, decimal? price, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            int role = tokenModel.Role;
            if (role != (int)RoleConstants.Admin)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");

            var priceList = await Get(temp => temp.AreaId.Equals(areaId) && temp.MotorTypeId.Equals(motorTypeId)).FirstOrDefaultAsync();
            priceList.Price = price;
            await UpdateAsync(priceList);
            return priceList;
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

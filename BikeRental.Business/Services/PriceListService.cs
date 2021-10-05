using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.RequestModels;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IPriceListService : IBaseService<PriceList>
    {
        Task<List<PriceListViewModel>> GetAll();
        Task<PriceList> Create(PricelistCreateRequest request);
        Task<PriceList> Update(Guid areaId, Guid cateId, decimal? price);
    }
    public class PriceListService : BaseService<PriceList>, IPriceListService
    {
        private readonly IConfigurationProvider _mapper;
        public PriceListService(IUnitOfWork unitOfWork, IPriceListRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
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

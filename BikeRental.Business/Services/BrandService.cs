using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        List<BrandViewModel> GetAll();
        BrandViewModel GetBrandById(Guid id);
    }
    public class BrandService : BaseService<Brand>, IBrandService
    {
        private readonly IConfigurationProvider _mapper;
        
        public BrandService(IUnitOfWork unitOfWork, IBrandRepository repository , IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public List<BrandViewModel> GetAll()
        {
            return Get().ProjectTo<BrandViewModel>(_mapper).ToList();
        }

        public BrandViewModel GetBrandById(Guid id)
        {
            return Get(b => b.Id.Equals(id)).ProjectTo<BrandViewModel>(_mapper).FirstOrDefault();
        }
    }
}

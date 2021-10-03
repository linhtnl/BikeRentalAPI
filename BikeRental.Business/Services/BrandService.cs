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

        Task<Brand> Update(Guid id, string name);

        Task<Brand> Create(string name);
    }
    public class BrandService : BaseService<Brand>, IBrandService
    {
        private readonly IConfigurationProvider _mapper;
        
        public BrandService(IUnitOfWork unitOfWork, IBrandRepository repository , IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Brand> Create(string name)
        {
            var brand = new Brand();
            brand.Name = name;
            await CreateAsync(brand);
            return brand;
        }

        public List<BrandViewModel> GetAll()
        {
            return Get().ProjectTo<BrandViewModel>(_mapper).ToList();
        }

        public BrandViewModel GetBrandById(Guid id)
        {
            return Get(b => b.Id.Equals(id)).ProjectTo<BrandViewModel>(_mapper).FirstOrDefault();
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

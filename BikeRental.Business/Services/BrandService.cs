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
        Task<DynamicModelResponse<BrandViewModel>> GetAll(BrandViewModel model, int pageNum);
        Task<BrandViewModel> GetBrandById(Guid? id);

        Task<Brand> Update(Guid id, string name);

        Task<Brand> Create(string name);
        Task<bool> Delete(Guid id);
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

        public async Task<bool> Delete(Guid id)
        {
            var brand = Get(b => b.Id.Equals(id)).FirstOrDefault();
            brand.Status = (int)BrandStatus.Unavailable;
            try
            {
                await UpdateAsync(brand);
                return true;
            }
            catch
            {
                return false;
            }          
        }

        public async Task<DynamicModelResponse<BrandViewModel>> GetAll(BrandViewModel model, int pageNum)
        {
            var listBrand = Get().ProjectTo<BrandViewModel>(_mapper)
                .DynamicFilter(model)
                .PagingIQueryable(pageNum, GlobalConstants.GROUP_ITEM_NUM, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
            if (listBrand.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var rs = new DynamicModelResponse<BrandViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = pageNum,
                    Size = GlobalConstants.GROUP_ITEM_NUM,
                    Total = listBrand.Item1
                },
                Data = await listBrand.Item2.ToListAsync()
            };
            return rs;
        }

        public async Task<BrandViewModel> GetBrandById(Guid? id)
        {
            var brand = await Get(b => b.Id.Equals(id)).ProjectTo<BrandViewModel>(_mapper).FirstOrDefaultAsync();
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

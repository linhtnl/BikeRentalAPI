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
    public interface ICategoryService : IBaseService<Category>
    {
        Task<DynamicModelResponse<CategoryViewModel>> GetAll(CategoryViewModel model, int pageNum);
        Task<CategoryViewModel> GetCateById(Guid? id);
        Task<Category> Update(Guid id, string name, int type);
        Task<Category> Create(CategoryCreateModel model);
        Task<bool> Delete(Guid id);
        Task<List<Category>> GetCateByBrandId(Guid id);
    }
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        private readonly IConfigurationProvider _mapper;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Category> Create(CategoryCreateModel model)
        {
            var cate = _mapper.CreateMapper().Map<Category>(model);
            await CreateAsync(cate);
            return cate;
        }

        public async Task<bool> Delete(Guid id)
        {
            var cate = Get(c => c.Id.Equals(id)).FirstOrDefault();
            cate.Status = (int)CategoryStatus.Unavailable;
            try
            {
                await UpdateAsync(cate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DynamicModelResponse<CategoryViewModel>> GetAll(CategoryViewModel model, int pageNum)
        {
            var categories = Get().ProjectTo<CategoryViewModel>(_mapper)
                 .DynamicFilter(model)
                 .PagingIQueryable(pageNum, GlobalConstants.GROUP_ITEM_NUM, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
            if (categories.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var rs = new DynamicModelResponse<CategoryViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = pageNum,
                    Size = GlobalConstants.GROUP_ITEM_NUM,
                    Total = categories.Item1
                },
                Data = await categories.Item2.ToListAsync()
            };
            return rs;
        }

        public async Task<List<Category>> GetCateByBrandId(Guid id)
        {
            return await Get(c => c.BrandId.Equals(id)).ToListAsync();
        }

        public async Task<CategoryViewModel> GetCateById(Guid? id)
        {
            var cate = await Get(c => c.Id.Equals(id)).ProjectTo<CategoryViewModel>(_mapper).FirstOrDefaultAsync();
            return cate;
        }

        public async Task<Category> Update(Guid id, string name, int type)
        {
            var cate = Get(g => g.Id.Equals(id)).FirstOrDefault();
            cate.Name = name;
            cate.Type = type;
            await UpdateAsync(cate);
            return cate;
        }
    }
}

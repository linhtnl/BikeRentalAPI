using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Enums;
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
    public interface ICategoryService : IBaseService<Category>
    {
        List<CategoryViewModel> GetAllCate();
        CategoryViewModel GetCateById(Guid id);
        List<CategoryViewModel> GetCateByType(int type);
        Task<Category> Update(Guid id, string name, int type);
        Task<Category> Create(CategoryCreateModel model);
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

        public List<CategoryViewModel> GetAllCate()
        {
            return Get().ProjectTo<CategoryViewModel>(_mapper).ToList();
        }

        public CategoryViewModel GetCateById(Guid id)
        {
            return Get(c => c.Id.Equals(id)).ProjectTo<CategoryViewModel>(_mapper).FirstOrDefault();
        }

        public List<CategoryViewModel> GetCateByType(int type)
        {
            return Get(c => c.Type == type).ProjectTo<CategoryViewModel>(_mapper).ToList();
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

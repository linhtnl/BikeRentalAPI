using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using BikeRental.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using BikeRental.Business.Utilities;
using Microsoft.EntityFrameworkCore;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;

namespace BikeRental.Business.Services
{
    public interface IBikeService : IBaseService<Bike>
    {
        Task<DynamicModelResponse<BikeViewModel>> GetAll(BikeViewModel model, int pageNum);
        Task<BikeViewModel> GetBikeById(Guid id);
        Task<BikeViewModel> Create(BikeCreateRequest request);
        Task<BikeViewModel> Update(Guid id , BikeUpdateRequest request);
        Task<Bike> Delete(Guid id);
        Task<List<Bike>> GetBikeByOwnerId(Guid id);
    }
    public class BikeService : BaseService<Bike>, IBikeService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly IFeedbackService _feedbackService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        public BikeService(IUnitOfWork unitOfWork, IBikeRepository repository,
            IFeedbackService feedbackService, ICategoryService categoryService,IBrandService brandService, IMapper mapper) : base(unitOfWork, repository)
        {
            _feedbackService = feedbackService;
            _categoryService = categoryService;
            _brandService = brandService;
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<BikeViewModel> GetBikeById(Guid id)
        {
            var bike = await Get(x => x.Id.Equals(id)).ProjectTo<BikeViewModel>(_mapper).FirstOrDefaultAsync();
            return bike;
        }

        public async Task<DynamicModelResponse<BikeViewModel>> GetAll(BikeViewModel model, int pageNum)
        {
            //if user = admin BikeStatus : show all
            //if user = owner BikeStatus : Available, Rent base on ownerID
            //if user = user BikeStatus : Available only
            var bikes = Get(x => x.Status == (int)BikeStatus.Available).ProjectTo<BikeViewModel>(_mapper);
            List<BikeViewModel> listBike = bikes.ToList();
            if(listBike.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            for (int i = 0; i < listBike.Count; i++)
            {
                var cate = await _categoryService.GetCateById(listBike[i].CategoryId);
                var brand = await _brandService.GetBrandById(cate.BrandId);
                listBike[i].BrandName = brand.Name;
            }
            bikes = listBike.AsQueryable().OrderByDescending(b => b.BrandName).ThenByDescending(b => b.CategoryName);
            var result = bikes
                .DynamicFilter(model)
                .PagingIQueryable(pageNum, GlobalConstants.GROUP_ITEM_NUM, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
            //if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var rs = new DynamicModelResponse<BikeViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = pageNum,
                    Size = GlobalConstants.GROUP_ITEM_NUM,
                    Total = result.Item1
                },
                Data = result.Item2.ToList()
            };
            return rs;
        }

        public async Task<BikeViewModel> Create(BikeCreateRequest request)
        {
            //get id from token
            var bike = _mapper.CreateMapper().Map<Bike>(request);
            var cate = await _categoryService.GetCateById(bike.CategoryId);
            if (cate.Status == (int)CategoryStatus.Pending)
            {
                bike.Status = (int)BikeStatus.Pending;
            }
            else
            {
                bike.Status = (int)BikeStatus.Available;
            }
            await CreateAsync(bike);
            var result = _mapper.CreateMapper().Map<BikeViewModel>(bike);
            return result;
        }

        public async Task<BikeViewModel> Update(Guid id, BikeUpdateRequest request)
        {
            var bike = await GetAsync(id);
            if (bike == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Bike not found");
            var updateBike = _mapper.CreateMapper().Map(request, bike);
            await UpdateAsync(updateBike);
            var result = _mapper.CreateMapper().Map<BikeViewModel>(updateBike);
            return result;
        }

        public async Task<Bike> Delete(Guid id)
        {
            var bike = await GetAsync(id);
            if (bike == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Bike not found");
            bike.Status = (int)BikeStatus.Delete;
            await UpdateAsync(bike);
            return bike;
        }

        public async Task<List<Bike>> GetBikeByOwnerId(Guid id)
        {
            return await Get(b => b.OwnerId.Equals(id)).ToListAsync();
        }
    }
}

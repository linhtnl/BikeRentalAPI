using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
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
    public interface IOwnerService : IBaseService<Owner>
    {
        Task<OwnerViewModel> CreateNew(Owner ownerInfo);
        Task<OwnerViewModel> Delete(Guid id);
        Task<OwnerDetailViewModel> GetOwnerById(Guid id);
        Task<Owner> GetOwner(Guid id);
        Task<OwnerViewModel> GetByMail(string mail);
        Task<DynamicModelResponse<OwnerRatingViewModel>> GetAll(OwnerRatingViewModel model, int filterOption, int pageNum);
    }
    public class OwnerService : BaseService<Owner>, IOwnerService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly IBikeService _bikeService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IFeedbackService _feedbackService;
        public OwnerService(IUnitOfWork unitOfWork,IBikeService bikeService,IFeedbackService feedbackService,
            ICategoryService categoryService, IBrandService brandService, IOwnerRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _bikeService = bikeService;
            _feedbackService = feedbackService;
            _categoryService = categoryService;
            _brandService = brandService;
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Owner> GetOwner(Guid id)
        {
            var owner = await GetAsync(id);
            return owner;
        }

        public async Task<OwnerDetailViewModel> GetOwnerById(Guid id)
        {
            int total = 0;
            double? rating = 0;
            var owner = Get(x => x.Id.Equals(id)).ProjectTo<OwnerDetailViewModel>(_mapper).FirstOrDefault();
            var listBike = await _bikeService.GetBikeByOwnerId(id);
            if (listBike.Count != 0)
            {
                owner.ListBike = _mapper.CreateMapper().Map<List<BikeViewModel>>(listBike);
                for (int i = 0; i < listBike.Count; i++)
                {
                    var cate = await _categoryService.GetCateById(listBike[i].CategoryId);
                    owner.ListBike[i].CategoryName = cate.Name;
                    var brand = await _brandService.GetBrandById(cate.BrandId);
                    owner.ListBike[i].BrandName = brand.Name;
                    var tempRating = await _feedbackService.GetBikeRating(listBike[i].Id);
                    total += tempRating.FirstOrDefault().Key;
                    rating += tempRating.FirstOrDefault().Value;
                }
                owner.Rating = rating / listBike.Count;
                owner.NumberOfRatings = total;
            }
            return owner;
        }

        public async Task<OwnerViewModel> GetByMail(string mail)
        {
            return await Get().Where(tempOwner => tempOwner.Mail.Equals(mail)).ProjectTo<OwnerViewModel>(_mapper).FirstOrDefaultAsync();
        }

        public async Task<OwnerViewModel> CreateNew(Owner ownerInfo)
        {
            try
            {
                await CreateAsync(ownerInfo);

                return await GetByMail(ownerInfo.Mail);
            }
            catch
            {
                return null;
            }
        }
        public async Task<DynamicModelResponse<OwnerRatingViewModel>> GetAll(OwnerRatingViewModel model, int filterOption, int pageNum)
        {
            double? rating = 0;
            int total = 0;
            var owners = Get(o => o.Bikes != null).ProjectTo<OwnerRatingViewModel>(_mapper);
            List<OwnerRatingViewModel> listOwner = owners.ToList();
            if (listOwner.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            for (int i = 0; i < listOwner.Count; i++)
            {
                var listBike = await _bikeService.GetBikeByOwnerId(Guid.Parse(listOwner[i].Id.ToString()));
                listOwner[i].NumberOfBikes = listBike.Count();
                foreach (var bike in listBike)
                {
                    var tempRating = await _feedbackService.GetBikeRating(bike.Id);
                    total += tempRating.FirstOrDefault().Key;
                    rating += tempRating.FirstOrDefault().Value;
                }
                listOwner[i].Rating = rating / listBike.Count();
                listOwner[i].NumberOfRatings = total;
            }
            if (filterOption == (int)OwnerFilterOptions.TotalBike)
            {
                owners = listOwner.AsQueryable().OrderByDescending(o => o.NumberOfBikes);
            }
            else if (filterOption == (int)OwnerFilterOptions.Rating)
            {
                owners = listOwner.AsQueryable().OrderByDescending(o => o.Rating);
            }
            else
            {
                owners = listOwner.AsQueryable().OrderByDescending(o => o.Rating).ThenByDescending(o => o.NumberOfBikes);
            }
            var result = owners
                .DynamicFilter(model)
                .PagingIQueryable(pageNum, GlobalConstants.GROUP_ITEM_NUM, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

            var rs = new DynamicModelResponse<OwnerRatingViewModel>
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

        public async Task<OwnerViewModel> Delete(Guid id)
        {
            //only admin
            var owner = await GetAsync(id);
            if (owner == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Bike not found");
            owner.Status = (int)UserStatus.Deactive;
            owner.BanTimes++;
            await UpdateAsync(owner);
            var result = _mapper.CreateMapper().Map<OwnerViewModel>(owner);
            return result;
        }
    }
}

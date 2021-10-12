using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IUtilService : IBaseService<Owner>
    {
        Task<List<OwnerByAreaViewModel>> GetListOwnerByAreaId(Guid id);
    }

    public class UtilService : BaseService<Owner>, IUtilService
    {
        private readonly IConfigurationProvider _mapper;

        private readonly IBikeService _bikeService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IBookingUtilService _feedbackService;

        public UtilService(IUnitOfWork unitOfWork, IOwnerRepository ownerRepository, IMapper mapper, 
            IBikeService bikeService, 
            ICategoryService categoryService, 
            IBrandService brandService,
            IBookingUtilService feedbackService) : base(unitOfWork, ownerRepository)
        {
            _mapper = mapper.ConfigurationProvider;

            _bikeService = bikeService;
            _categoryService = categoryService;
            _brandService = brandService;
            _feedbackService = feedbackService;
        }

        public async Task<List<OwnerByAreaViewModel>> GetListOwnerByAreaId(Guid id)
        {
            int total = 0;
            double? rating = 0;
            var owners = Get(x => x.AreaId.Equals(id)).ProjectTo<OwnerByAreaViewModel>(_mapper);
            var listOwner = owners.ToList();
            if (listOwner.Count == 0) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Can not found");
            for (int i = 0; i < listOwner.Count; i++)
            {
                var listBike = await _bikeService.GetBikeByOwnerId(listOwner[i].Id);
                if (listBike.Count != 0)
                {
                    listOwner[i].ListBike = _mapper.CreateMapper().Map<List<BikeViewModel>>(listBike);
                    for (int j = 0; j < listBike.Count; j++)
                    {
                        var cate = await _categoryService.GetCateById(listBike[j].CategoryId);
                        listOwner[i].ListBike[j].CategoryName = cate.Name;
                        var brand = await _brandService.GetBrandById(cate.BrandId);
                        listOwner[i].ListBike[j].BrandName = brand.Name;
                        var tempRating = await _feedbackService.GetBikeRating(listBike[j].Id);
                        if (tempRating.FirstOrDefault().Value != 0)
                        {
                            total += tempRating.FirstOrDefault().Key;
                            rating += tempRating.FirstOrDefault().Value;
                        }
                    }
                    if (total != 0)
                    {
                        listOwner[i].Rating = rating / total;
                    }
                    else
                    {
                        listOwner[i].Rating = 0;
                    }
                }
            }
            var result = listOwner.ToList();
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].ListBike == null)
                {
                    result.RemoveAt(i);
                    i--;
                }
            }
            if (result[0].ListBike == null)
            {
                result.RemoveAt(0);
            }
            return result;
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public interface IBikeUtilService : IBaseService<Bike>
    {
        Task<BikeFindingViewModel> FindBike(Guid ownerId, Guid typeId, double price);
    }
    public class BikeUtilService : BaseService<Bike>, IBikeUtilService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IFeedbackService _feedbackService;
        private readonly ICategoryService _categoryService;
        private readonly IWalletUtilService _walletUtilService;
        public BikeUtilService(IUnitOfWork unitOfWork, IBikeRepository repository,
            IFeedbackService feedbackService,
            ICategoryService categoryService,
            IWalletUtilService walletUtilService,
            IMapper mapper) : base(unitOfWork, repository)
        {
            _feedbackService = feedbackService;
            _categoryService = categoryService;
            _walletUtilService = walletUtilService;
            _mapper = mapper.ConfigurationProvider;

        }

        public async Task<BikeFindingViewModel> FindBike(Guid ownerId, Guid typeId, double price)
        {

            var suitableBalance = (price * 10) / 100;
            var listBike = await Get(b => b.OwnerId.Equals(ownerId) && b.Status == (int)BikeStatus.Available).ProjectTo<BikeFindingViewModel>(_mapper).ToListAsync();
            if (listBike.Count != 0)
            {

                foreach (var bike in listBike)
                {
                    bike.TotalBike = listBike.Count;
                    var cate = await _categoryService.GetCateById(bike.CategoryId);
                    bike.CateName = cate.Name;
                    var rating = await _feedbackService.GetBikeRating(Guid.Parse(bike.Id.ToString()));
                    if (rating.FirstOrDefault().Value != 0)
                    {
                        bike.NumberOfRating = rating.FirstOrDefault().Key;
                        bike.Rating = rating.FirstOrDefault().Value;
                    }
                    else
                    {
                        bike.NumberOfRating = 0;
                        bike.Rating = 0;
                    }
                }
                var listSuitableBike = listBike.ToList();
                for (int i = 0; i < listSuitableBike.Count; i++)
                {
                    var cate = await _categoryService.GetCateById(listSuitableBike[i].CategoryId);
                    var balance = await _walletUtilService.GetWalletBalance(Guid.Parse(listSuitableBike[i].OwnerId.ToString()));
                    if (!cate.MotorTypeId.Equals(typeId) || (double)balance < suitableBalance)
                    {
                        listSuitableBike.RemoveAt(i);
                        i--;
                    }
                }
                if (listSuitableBike.Count == 0) return null;
                var cateTemp = await _categoryService.GetCateById(listSuitableBike[0].CategoryId);
                if (!cateTemp.MotorTypeId.Equals(typeId))
                {
                    listSuitableBike.RemoveAt(0);
                }
                var result = listSuitableBike.AsQueryable().OrderByDescending(b => b.Rating);
                var rs = result.ToList();
                return rs.FirstOrDefault();
            }
            else return null;
        }
    }
}

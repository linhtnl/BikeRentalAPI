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
        Task<BikeFindingViewModel> FindBike(Guid ownerId, Guid typeId);
    }
    public class BikeUtilService : BaseService<Bike>, IBikeUtilService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IFeedbackService _feedbackService;
        private readonly ICategoryService _categoryService;
        public BikeUtilService(IUnitOfWork unitOfWork, IBikeRepository repository,
            IFeedbackService feedbackService,
            ICategoryService categoryService,
            IMapper mapper) : base(unitOfWork, repository)
        {
            _feedbackService = feedbackService;
            _categoryService = categoryService;
            _mapper = mapper.ConfigurationProvider;
         
        }

        public async Task<BikeFindingViewModel> FindBike(Guid ownerId, Guid typeId)
        {
            var listBike = await Get(b => b.OwnerId.Equals(ownerId) && b.Status == (int)BikeStatus.Available).ProjectTo<BikeFindingViewModel>(_mapper).ToListAsync();
            if (listBike.Count() != 0)
            {
                foreach (var bike in listBike)
                {                        
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
                for(int i = 0; i < listSuitableBike.Count; i++)
                {
                    var cate = await _categoryService.GetCateById(listSuitableBike[i].CategoryId);
                    if (!cate.MotorTypeId.Equals(typeId))
                    {
                        listSuitableBike.RemoveAt(i);
                        i--;
                    }
                }
                if (listSuitableBike.Count==0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
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

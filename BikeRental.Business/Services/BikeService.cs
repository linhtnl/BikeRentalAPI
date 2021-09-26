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

namespace BikeRental.Business.Services
{
    public interface IBikeService : IBaseService<Bike>
    {
        /*Task<DynamicModelResponse<BikeViewModel>> GetAll(BikeViewModel model, string[] fields, int page, int size);*/

        BikeViewModel GetBikeById(Guid id);
    }
    public class BikeService : BaseService<Bike>, IBikeService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly IOwnerService _ownerService;
        private readonly IFeedbackService _feedbackService;
        public BikeService(IUnitOfWork unitOfWork, IBikeRepository repository,
            IOwnerService ownerService,IFeedbackService feedbackService, IMapper mapper) : base(unitOfWork, repository)
        {
            _ownerService = ownerService;
            _mapper = mapper.ConfigurationProvider;
        }

        public BikeViewModel GetBikeById(Guid id)
        {
            return Get(x => x.Id.Equals(id)).ProjectTo<BikeViewModel>(_mapper).FirstOrDefault();
        }

        /*public async Task<DynamicModelResponse<BikeViewModel>> GetAll(BikeViewModel model, string[] fields, int page, int size)
        {
            var bikes = Get(x => x.Status == (int)BikeStatus.Available).ProjectTo<BikeViewModel>(_mapper);

            List<BikeViewModel> listBike = bikes.ToList();
            if (listBike.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found!");
            for (int i = 0; i < listBike.Count; i++)
            {
                var owner = await _ownerService.GetOwner((Guid)listBike[i].OwnerId);
                if (owner != null)
                {
                    listBike[i].OwnerName = owner.Fullname;
                    listBike[i].OwnerPhone = owner.PhoneNumber;
                }
                var ratings = await _feedbackService.GetBikeRating((Guid)listBike[i].Id);
                listBike[i].Rating = ratings.FirstOrDefault().Value;
                listBike[i].Rating = ratings.FirstOrDefault().Key;
            }
            bikes = listBike.AsQueryable().OrderByDescending(b => b.Rating);

            var result = ((IQueryable)bikes)
        }*/
    }
}

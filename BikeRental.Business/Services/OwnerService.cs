using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        Task<OwnerRatingViewModel> GetOwnerById(Guid id);
        Task<Owner> GetOwner(Guid id);
        Task<OwnerViewModel> GetByMail(string mail);
        Task<List<OwnerRatingViewModel>> GetAll();
    }
    public class OwnerService : BaseService<Owner>, IOwnerService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly IBikeService _bikeService;
        private readonly IFeedbackService _feedbackService;
        public OwnerService(IUnitOfWork unitOfWork,IBikeService bikeService,IFeedbackService feedbackService, IOwnerRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _bikeService = bikeService;
            _feedbackService = feedbackService;
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Owner> GetOwner(Guid id)
        {
            var owner = await GetAsync(id);
            return owner;
        }

        public async Task<OwnerRatingViewModel> GetOwnerById(Guid id)
        {
            return await Get(x => x.Id.Equals(id)).ProjectTo<OwnerRatingViewModel>(_mapper).FirstOrDefaultAsync();
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
        public async Task<List<OwnerRatingViewModel>> GetAll()
        {
            double? rating = 0;
            int total = 0;
            var owners = Get(o => o.Bikes != null).ProjectTo<OwnerRatingViewModel>(_mapper);
            List<OwnerRatingViewModel> listOwner = owners.ToList();
            if (listOwner.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            for (int i = 0; i < listOwner.Count; i++)
            {
                var listBike = await _bikeService.GetBikeByOwnerId(listOwner[i].Id);
                listOwner[i].NumberOfBikes = listBike.Count();
                foreach (var bike in listBike)
                {
                    var tempRating = await _feedbackService.GetBikeRating(bike.Id);
                    total += tempRating.FirstOrDefault().Key;
                    rating += tempRating.FirstOrDefault().Value;
                }
                listOwner[i].Rating = rating;
                listOwner[i].NumberOfRatings = total;
            }
            return listOwner;
        }
    }
}

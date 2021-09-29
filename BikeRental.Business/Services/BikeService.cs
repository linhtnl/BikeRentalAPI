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
using BikeRental.Data.Constants;

namespace BikeRental.Business.Services
{
    public interface IBikeService : IBaseService<Bike>
    {
        List<BikeViewModel> GetAll();

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
            _feedbackService = feedbackService;
            _mapper = mapper.ConfigurationProvider;
        }

        public BikeViewModel GetBikeById(Guid id)
        {
            return Get(x => x.Id.Equals(id)).ProjectTo<BikeViewModel>(_mapper).FirstOrDefault();
        }

        public List<BikeViewModel> GetAll()
        {
            return Get(x => x.Status == (int)BikeStatus.Rent).ProjectTo<BikeViewModel>(_mapper).ToList();
        }
    }
}

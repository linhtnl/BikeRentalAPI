using AutoMapper;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IFeedbackService : IBaseService<Feedback>
    {
        Task<Dictionary<int, double?>> GetBikeRating(Guid bikeId);
    }
    public class FeedbackService : BaseService<Feedback>, IFeedbackService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly IBikeRepository _bikeRepository;
        public FeedbackService(IUnitOfWork unitOfWork, IFeedbackRepository repository, IMapper mapper
            ,IBikeRepository bikeRepository) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _bikeRepository = bikeRepository;
        }

        public async Task<Dictionary<int, double?>> GetBikeRating(Guid bikeId)
        {
            var feedbacks = await Get(x => x.IdNavigation.BikeId.Equals(bikeId)).ToListAsync();
            Dictionary<int, double?> result = new Dictionary<int, double?>();
            result.Add(0, null);
            if (feedbacks.Count == 0) return result;
            result.Clear();
            double rating = 0;
            int total = 0;
            foreach(var feedback in feedbacks)
            {
                total++;
                rating = +(float)feedback.Rating;
            }
            result.Add(total, rating / total);
            return result;
        }
    }
}

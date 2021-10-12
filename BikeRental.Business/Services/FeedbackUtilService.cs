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
    public interface IFeedbackUtilService : IBaseService<Feedback>
    {
        Task<Feedback> GetFeedbackByBookingId(Guid id);
    }

    public class FeedbackUtilService : BaseService<Feedback>, IFeedbackUtilService
    {
        private readonly IConfigurationProvider _mapper;

        public FeedbackUtilService(IUnitOfWork unitOfWork, IFeedbackRepository feedbackRepository, IMapper mapper) : base(unitOfWork, feedbackRepository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Feedback> GetFeedbackByBookingId(Guid id)
        {
            return await Get(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }
    }
}

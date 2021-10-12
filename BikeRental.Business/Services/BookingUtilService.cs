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
    public interface IBookingUtilService : IBaseService<Booking>
    {
        Task<Dictionary<int, double?>> GetBikeRating(Guid bikeId);
    }
    public class BookingUtilService : BaseService<Booking>, IBookingUtilService
    {
        private readonly IConfigurationProvider _mapper;

        private readonly IFeedbackUtilService _feedbackService;

        public BookingUtilService(IUnitOfWork unitOfWork, IBookingRepository bookingRepository, IMapper mapper, 
            IFeedbackUtilService feedbackService) : base(unitOfWork, bookingRepository)
        {
            _mapper = mapper.ConfigurationProvider;

            _feedbackService = feedbackService;
        }

        public async Task<List<Booking>> GetByBikeId(Guid id)
        {
            return await Get(b => b.BikeId.Equals(id)).ToListAsync();
        }

        public async Task<Dictionary<int, double?>> GetBikeRating(Guid bikeId)
        {
            Dictionary<int, double?> result = new Dictionary<int, double?>();
            result.Add(0, 0);
            var listFeedback = new List<Feedback>();
            var bookings = await GetByBikeId(bikeId);
            if (bookings.Count == 0)
            {
                return result;
            }
            foreach (var booking in bookings)
            {
                Guid bookingId = booking.Id;
                var feedback = await _feedbackService.GetFeedbackByBookingId(bookingId);
                listFeedback.Add(feedback);
            }
            if (listFeedback.Count == 0) return result;
            result.Clear();
            double rating = 0;
            int total = 0;
            foreach (var feedback in listFeedback)
            {
                if (feedback != null)
                {
                    total++;
                    rating += (float)feedback.Rating;
                }
            }

            result.Add(total, rating / total);
            return result;

        }
    }
}

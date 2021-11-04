using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Data.Enums;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IFeedbackService : IBaseService<Feedback>
    {
        Task<Dictionary<int, double?>> GetBikeRating(Guid bikeId);
        Task<List<FeedbackViewModel>> GetListFeedBackByOwnerId(String token);
        Task<Feedback> Create(FeedbackCreateRequest request);
        Task<Feedback> Update(FeedbackCreateRequest request);
    }
    public class FeedbackService : BaseService<Feedback>, IFeedbackService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;
        private readonly IBookingService _bookingService;
        private readonly ICustomerService _customerService;

        public FeedbackService(IUnitOfWork unitOfWork, IFeedbackRepository repository, IMapper mapper, IConfiguration configuration
            , IBookingService bookingService, ICustomerService customerService) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _bookingService = bookingService;
            _customerService = customerService;
        }

        public async Task<Feedback> Create(FeedbackCreateRequest request)
        {
            var booking = await _bookingService.GetById(request.Id);
            if (booking == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Booking not found");
            try
            {
                var feedback = _mapper.CreateMapper().Map<Feedback>(request);
                await CreateAsync(feedback);
                return feedback;
            }
            catch (Exception)
            {
                throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "Invalid Data");
            }


        }

        public async Task<Dictionary<int, double?>> GetBikeRating(Guid bikeId)
        {
            Dictionary<int, double?> result = new Dictionary<int, double?>();
            result.Add(0, 0);
            var listFeedback = new List<Feedback>();
            var bookings = await _bookingService.GetByBikeId(bikeId);
            if (bookings.Count == 0)
            {
                return result;
            }
            foreach (var booking in bookings)
            {
                var feedback = await Get(x => x.Id.Equals(booking.Id)).FirstOrDefaultAsync();
                if (feedback != null)
                {
                    listFeedback.Add(feedback);
                }
            }
            if (listFeedback.Count == 0) return result;
            result.Clear();
            double rating = 0;
            int total = 0;
            foreach (var feedback in listFeedback)
            {
                if (feedback != null)
                {
                    if (feedback.Rating==null || feedback.Rating > 0)
                    {
                        total++;
                        rating += (float)feedback.Rating;
                    }
                }
            }

            result.Add(total, rating / total);
            return result;

        }

        public async Task<List<FeedbackViewModel>> GetListFeedBackByOwnerId(string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);
            int role = tokenModel.Role;
            if(role != (int)RoleConstants.Owner) throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");

            Guid ownerId = tokenModel.Id;
            var listBooking = await _bookingService.GetListBookingByOwnerId(ownerId);
            List<FeedbackViewModel> listFeedback = new List<FeedbackViewModel>();
            foreach(var booking in listBooking)
            {
                var feedback = await Get(b => b.Id.Equals(booking.Id)).ProjectTo<FeedbackViewModel>(_mapper).FirstOrDefaultAsync();
                if(feedback != null)
                {
                    if (feedback.Rating > 0)
                    {
                        var customer = await _customerService.GetCustomerById(Guid.Parse(booking.CustomerId.ToString()));
                        feedback.CustomerName = customer.Fullname;
                        feedback.Status = (int)FeedbackStatus.Feedback;
                        listFeedback.Add(feedback);

                    }else if (feedback.Rating == 0)
                    {
                        var customer = await _customerService.GetCustomerById(Guid.Parse(booking.CustomerId.ToString()));
                        feedback.CustomerName = customer.Fullname;
                        feedback.Status = (int)FeedbackStatus.Feedback;
                        listFeedback.Add(feedback);
                    }
                }
            }
            if(listFeedback.Count==0) throw new ErrorResponse((int)HttpStatusCode.NoContent, "Empty");
            return listFeedback;
        }

        public async Task<Feedback> Update(FeedbackCreateRequest request)
        {
            var feedback = await Get(f => f.Id.Equals(request.Id)).FirstOrDefaultAsync();
            if (feedback == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Feedback not found");
            var updateFeedback = _mapper.CreateMapper().Map(request, feedback);
            await UpdateAsync(updateFeedback);
            return updateFeedback;
        }
    }
}

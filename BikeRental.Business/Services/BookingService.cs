using AutoMapper;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IBookingService : IBaseService<Booking>
    {
        Task<BookingViewModel> GetAll();
    }
    public class BookingService : BaseService<Booking>, IBookingService
    {
        private readonly IConfigurationProvider _mapper;

        public BookingService(IUnitOfWork unitOfWork, IBookingRepository bookingRepository, IMapper mapper) : base(unitOfWork, bookingRepository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public Task<BookingViewModel> GetAll()
        {
            // token => role
            //if role = Customer/ Owner => GetAll booking base on that role
            //if role = Admin => Get All booking
            
            /*var customer 
            var bookings = Get(b => b.CustomerId.Equals)*/

            return null;
        }
    }
}

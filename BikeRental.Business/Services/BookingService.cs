using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Utilities;
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
    public interface IBookingService : IBaseService<Booking>
    {
        Task<List<BookingViewModel>> GetById(Guid id);
        Task<List<BookingViewModel>> GetAll();
        Task<BookingSuccessViewModel> CreateNew(TokenViewModel tokenModel, BookingCreateRequest model);
    }
    public class BookingService : BaseService<Booking>, IBookingService
    {
        private readonly IConfigurationProvider _mapper;

        private readonly IBikeService _bikeService;
        private readonly ICustomerService _customerService;
        private readonly IVoucherItemService _voucherItemService;

        public BookingService(IUnitOfWork unitOfWork, IBookingRepository bookingRepository, IMapper mapper,
            IBikeService bikeService,
            ICustomerService customerService,
            IVoucherItemService voucherItemService) : base(unitOfWork, bookingRepository)
        {
            _mapper = mapper.ConfigurationProvider;

            _bikeService = bikeService;
            _customerService = customerService;
            _voucherItemService = voucherItemService;
        }

        public Task<BookingSuccessViewModel> CreateNew(TokenViewModel tokenModel, BookingCreateRequest model)
        {
            Guid id = tokenModel.Id;


            return null;

        }

        public async Task<List<BookingViewModel>> GetAll()
        {
            var bookings = await Get().ProjectTo<BookingViewModel>(_mapper).ToListAsync();
            if(bookings.Count==0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            return bookings;
        }

        public async Task<List<BookingViewModel>> GetById(Guid id)
        {
            // token => role
            //if role = Customer/ Owner => GetAll booking base on that role
            //if role = Admin => Get All booking

            if (Get(b => b.CustomerId.Equals(id)).ProjectTo<BookingViewModel>(_mapper).Count()!=0)
            {
                var bookings = Get(b => b.CustomerId.Equals(id)).ProjectTo<BookingViewModel>(_mapper).ToList();
                return bookings;
            }
            else if(Get(b => b.OwnerId.Equals(id)).ProjectTo<BookingViewModel>(_mapper).Count() != 0)
            {
                var bookings = Get(b => b.OwnerId.Equals(id)).ProjectTo<BookingViewModel>(_mapper).ToList();

                return bookings;
            }
            else throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
        }
    }
}

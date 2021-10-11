using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Utilities;
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
    public interface IBookingService : IBaseService<Booking>
    {
        Task<List<BookingViewModel>> GetById(Guid id);
        Task<List<BookingViewModel>> GetAll();
        
        Task<List<Booking>> GetByBikeId(Guid id);
        Task<BookingSuccessViewModel> CreateNew(string token, BookingCreateRequest model);
        Task<BookingSuccessViewModel> UpdateStatus(string token, BookingUpdateStatusRequest request);
    }
    public class BookingService : BaseService<Booking>, IBookingService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;

        private readonly IBikeService _bikeService;
        private readonly ICustomerService _customerService;
        private readonly IVoucherItemService _voucherItemService;
        private readonly IVoucherService _voucherService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration,
            IBookingRepository bookingRepository,
            IBikeService bikeService,
            ICustomerService customerService,
            IVoucherItemService voucherItemService,
            IVoucherService voucherService
            ) : base(unitOfWork, bookingRepository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _bikeService = bikeService;
            _customerService = customerService;
            _voucherItemService = voucherItemService;
            _voucherService = voucherService;
        }

        public async Task<BookingSuccessViewModel> CreateNew(string token, BookingCreateRequest request)
        {
            //price lấy từ pricelist
            Voucher voucher = null;
            decimal? newPrice = null;
            bool isVoucherUsed = false;

            TokenViewModel tokenModel = new TokenService(_configuration).ReadJWTTokenToModel(token);

            Guid customerId = tokenModel.Id;
            int role = tokenModel.Role;
            if (role != (int)RoleConstants.Customer) throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This role cannot use this feature");

            if (request.VoucherCode != null)
            {
                var voucherItem = await _voucherItemService.GetAsync(request.VoucherCode);
                if (voucherItem == null)
                {
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Voucher code is not existed.");
                }
                else if (voucherItem.TimeUsingRemain <= 0)
                {
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Voucher is out of using time.");
                }

                voucher = await _voucherService.GetAsync(voucherItem.VoucherId);
            }

            if (voucher != null)
            {
                if (DateTime.Compare(DateTime.Today, voucher.ExpiredDate) > 0)
                {
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Voucher is expired.");
                } else if (DateTime.Compare(DateTime.Today, voucher.StartingDate) < 0)
                {
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Voucher's not started yet.");
                }

                newPrice = DiscountBooking(request.Price.Value, voucher.DiscountPercent.Value, voucher.DiscountAmount.Value);
            }

            if (newPrice != null)
            {
                request.Price = newPrice;
                isVoucherUsed = true;
            }

            if (request.DayRent == null)
            {
                request.DayRent = DateTime.Today;
            }

            if (isVoucherUsed)
            {
                var voucherItem = await _voucherItemService.GetAsync(request.VoucherCode);
                voucherItem.TimeUsingRemain -= 1;
                await _voucherItemService.UpdateAsync(voucherItem);
            }

            var bikeTarget = await _bikeService.GetAsync(request.BikeId);
            if (bikeTarget == null) throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Bike is not existed.");

            bikeTarget.Status = (int)BikeStatus.Pending;

            await _bikeService.UpdateAsync(bikeTarget);

            var targetBooking = _mapper.CreateMapper().Map<Booking>(request);

            targetBooking.OwnerId = bikeTarget.OwnerId;
            targetBooking.Status = (int)BookingStatus.Pending;

            await CreateAsync(targetBooking);

            var bookingResult = _mapper.CreateMapper().Map<BookingSuccessViewModel>(targetBooking);

            return await Task.Run(() => bookingResult);
        }
        
        private decimal? DiscountBooking(decimal originalPrice, int discountPercent, decimal maxDiscountAmount)
        {
            decimal discountPercentDecimal = discountPercent;

            var discountAmount = Decimal.Multiply(originalPrice, (Decimal.Divide(discountPercentDecimal, 100)));

            if (discountAmount >= maxDiscountAmount)
            {
                return maxDiscountAmount;
            } else
            {
                return originalPrice - discountAmount;
            }
        }

        public async Task<List<BookingViewModel>> GetAll()
        {
            var bookings = await Get().ProjectTo<BookingViewModel>(_mapper).ToListAsync();
            if(bookings.Count==0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            return bookings;
        }

        public async Task<List<Booking>> GetByBikeId(Guid id)
        {
            return await Get(b => b.BikeId.Equals(id)).ToListAsync();
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

        public async Task<BookingSuccessViewModel> UpdateStatus(string token, BookingUpdateStatusRequest request)
        {
            //customer update status => cancel booking 
            //owner update status => cancel booking / finish booking(lấy time hiện tại set làm day return actual)
            TokenViewModel tokenModel = new TokenService(_configuration).ReadJWTTokenToModel(token);

            if (tokenModel.Role != (int)RoleConstants.Customer && tokenModel.Role != (int)RoleConstants.Owner)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This role cannot use this feature");

            var booking = await GetAsync(request.Id);

            if (booking == null) throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Booking is not existed.");

            booking.Status = request.Status;
            await UpdateAsync(booking);

            BookingSuccessViewModel bookingResult = _mapper.CreateMapper().Map<BookingSuccessViewModel>(booking);
            return await Task.Run(() => bookingResult);
        }
    }
}

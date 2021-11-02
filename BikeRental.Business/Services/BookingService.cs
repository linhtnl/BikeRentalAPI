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
        private readonly IVoucherItemService _voucherItemService;
        private readonly IUtilService _utilService;
        private readonly IWalletService _walletService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration,
            IBookingRepository bookingRepository,
            IBikeService bikeService,
            IVoucherItemService voucherItemService,
            IUtilService utilService,
            IWalletService walletService
            ) : base(unitOfWork, bookingRepository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _bikeService = bikeService;
            _voucherItemService = voucherItemService;
            _utilService = utilService;
            _walletService = walletService;
        }

        public async Task<BookingSuccessViewModel> CreateNew(string token, BookingCreateRequest request)
        {
            Booking targetBooking = _mapper.CreateMapper().Map<Booking>(request);

            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            int role = tokenModel.Role;
            if (role != (int)RoleConstants.Customer)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature");

            Guid customerId = tokenModel.Id;

            targetBooking.CustomerId = customerId;

            Owner owner = await _utilService.GetOwnerByOwnerId(request.OwnerId);

            targetBooking.Price = request.Price;

            if (request.VoucherCode != null)
            {
                var voucherItemUsed = await _voucherItemService.GetAsync(request.VoucherCode);

                if (voucherItemUsed.TimeUsingRemain <= 0)
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This voucher code is out of using time.");

                voucherItemUsed.TimeUsingRemain -= 1;
                await _voucherItemService.UpdateAsync(voucherItemUsed);
            }

            var chosenBike = await _bikeService.GetAsync(request.BikeId);
            chosenBike.Status = (int)BikeStatus.Rent;
            await _bikeService.UpdateAsync(chosenBike);

            if (DateTime.Compare(request.DayRent, DateTime.Today) > 0)
            {
                targetBooking.Status = (int)BookingStatus.Pending;
            }

            await CreateAsync(targetBooking);

            var bookingResult = _mapper.CreateMapper().Map<BookingSuccessViewModel>(targetBooking);

            string dateTimeFormat = "yyyy-MM-dd";

            await TrackingBookingUtil.UpdateTrackingBooking(request.OwnerId, 
                Convert.ToDateTime(request.DayRent.ToString(dateTimeFormat)), 
                Convert.ToDateTime(request.DayReturnExpected.ToString(dateTimeFormat)));

            await _walletService
                .UpdateAmount(request.OwnerId, Decimal.Divide(request.Price, 10), (int)WalletStatus.DECREASE, bookingResult.Id);

            return await Task.Run(() => bookingResult);
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
                var bookings = await Get(b => b.CustomerId.Equals(id)).ProjectTo<BookingViewModel>(_mapper).ToListAsync();
                return bookings;
            }
            else if(Get(b => b.OwnerId.Equals(id)).ProjectTo<BookingViewModel>(_mapper).Count() != 0)
            {
                var bookings = await Get(b => b.OwnerId.Equals(id)).ProjectTo<BookingViewModel>(_mapper).ToListAsync();

                return bookings;
            }
            else throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
        }

        public async Task<BookingSuccessViewModel> UpdateStatus(string token, BookingUpdateStatusRequest request)
        {
            //customer update status => cancel booking 
            //owner update status => cancel booking / finish booking(lấy time hiện tại set làm day return actual)
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            int role = tokenModel.Role;

            if (role != (int)RoleConstants.Customer && role != (int)RoleConstants.Owner)
                throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");

            Guid userId = tokenModel.Id;

            int requestStatus = request.Status;

            if (!Enum.IsDefined(typeof(BookingStatus), requestStatus))
                throw new ErrorResponse((int)HttpStatusCode.NotImplemented, "This status is not supported yet.");

            var targetBooking = await GetAsync(request.Id);

            if ((role == (int)RoleConstants.Customer && !targetBooking.CustomerId.Equals(userId))
                || (role == (int)RoleConstants.Owner && !targetBooking.OwnerId.Equals(userId)))
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "You can not edit other's booking.");

            if (targetBooking == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This booking is not found.");

            if (targetBooking.Status == (int)BookingStatus.Finished || targetBooking.Status == (int)BookingStatus.Canceled)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This booking has done, so that, it can not be updated anymore.");

            //targetBooking.Status = request.Status;

            Bike targetBike = await _bikeService.GetAsync(targetBooking.BikeId);
            bool isUpdated = false;

            switch (request.Status)
            {
                case (int)BookingStatus.Pending:
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot update booking status to pending.");

                case (int)BookingStatus.Inprocess:
                    if (role == (int)RoleConstants.Customer)
                        throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");

                    targetBike.Status = (int)BikeStatus.Rent;

                    isUpdated = true;
                    break;

                case (int)BookingStatus.Finished:
                    if (role == (int)RoleConstants.Customer)
                        throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");

                    targetBike.Status = (int)BikeStatus.Available;

                    isUpdated = true;
                    break;

                case (int)BookingStatus.Canceled:
                    if (role == (int)RoleConstants.Customer && targetBooking.Status != (int)BookingStatus.Pending)
                        throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");

                    targetBike.Status = (int)BikeStatus.Available;

                    isUpdated = true;
                    break;

                default:
                    throw new ErrorResponse((int)HttpStatusCode.NotImplemented, "This status has not implemented yet.");
            }

            if (isUpdated)
            {
                targetBooking.DayReturnActual = DateTime.Today;
                targetBooking.Status = request.Status;

                await _bikeService.UpdateAsync(targetBike);
                await UpdateAsync(targetBooking);
            }

            BookingSuccessViewModel resultBooking = _mapper.CreateMapper().Map<BookingSuccessViewModel>(targetBooking);

            return await Task.Run(() => resultBooking);
        }
    }
}

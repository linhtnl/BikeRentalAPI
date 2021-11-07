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
        Task<BookingViewModel> GetById(Guid id);
        Task<DynamicModelResponse<BookingViewModel>> GetAll(string token, int status, int size, int pageNum);
        Task<List<BookingViewModel>> GetListBookingByOwnerId(Guid id);
        Task<List<Booking>> GetByBikeId(Guid id);
        Task<BookingSuccessViewModel> CreateNew(string token, BookingCreateRequest model);
        Task<BookingSuccessViewModel> UpdateStatus(string token, BookingUpdateStatusRequest request);
        Task<BookingDetailViewModel> GetBookingDetailById(Guid id, string token);
        Task<bool> SaveBookingEvidence(Guid id, string path);
    }
    public class BookingService : BaseService<Booking>, IBookingService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;

        private readonly IBikeService _bikeService;
        private readonly IVoucherItemService _voucherItemService;
        private readonly IUtilService _utilService;
        private readonly IWalletService _walletService;
        private readonly ICustomerService _customerService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration,
            IBookingRepository bookingRepository,
            IBikeService bikeService,
            IVoucherItemService voucherItemService,
            IUtilService utilService,
            IWalletService walletService,
            ICustomerService customerService

            ) : base(unitOfWork, bookingRepository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _bikeService = bikeService;
            _voucherItemService = voucherItemService;
            _utilService = utilService;
            _walletService = walletService;
            _customerService = customerService;
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

            if (!string.IsNullOrEmpty(request.StrVoucherCode.Trim()))
            {
                var tempGuid = Guid.Parse(request.StrVoucherCode);
                var voucherItemUsed = await _voucherItemService.GetAsync(tempGuid);

                if (voucherItemUsed.TimeUsingRemain <= 0)
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This voucher code is out of using time.");

                voucherItemUsed.TimeUsingRemain -= 1;
                await _voucherItemService.UpdateAsync(voucherItemUsed);

                targetBooking.VoucherCode = Guid.Parse(request.StrVoucherCode);
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

            var advanceMoney = Decimal.Divide(request.Price, 10);

            var check = await TrackingBookingTimeUtil.UpdateBookingTime(bookingResult.Id, advanceMoney);

            if (check == false) throw new ErrorResponse((int)HttpStatusCode.InternalServerError, "SomeThing went wrong");

            await _walletService
                .UpdateAmount(request.OwnerId, advanceMoney, (int)WalletStatus.DECREASE, bookingResult.Id);

            return await Task.Run(() => bookingResult);
        }

        public async Task<DynamicModelResponse<BookingViewModel>> GetAll(string token, int status, int size, int pageNum)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            int role = tokenModel.Role;
            if (role == (int)RoleConstants.Admin)
            {
                var bookings = await Get().ProjectTo<BookingViewModel>(_mapper).ToListAsync();
                if (bookings.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                foreach (var booking in bookings)
                {
                    var bike = await _bikeService.GetBikeById(Guid.Parse(booking.BikeId.ToString()));
                    bike = booking.Bike;
                }
                var result = bookings.AsQueryable().PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
                if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                var rs = new DynamicModelResponse<BookingViewModel>
                {
                    Metadata = new PagingMetaData
                    {
                        Page = pageNum,
                        Size = size,
                        Total = result.Item1
                    },
                    Data = result.Item2.ToList()
                };
                return rs;
            }
            else if (role == (int)RoleConstants.Owner)
            {
                Guid ownerId = tokenModel.Id;
                if (status == (int)BookingHistoryStatus.Ongoing)
                {
                    var bookings = await Get(b => b.OwnerId.Equals(ownerId)).
                        Where(b => b.Status == (int)BookingStatus.Pending || b.Status == (int)BookingStatus.Inprocess)
                        .ProjectTo<BookingViewModel>(_mapper).ToListAsync();
                    if (bookings.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                    foreach (var booking in bookings)
                    {
                        var bike = await _bikeService.GetBikeById(Guid.Parse(booking.BikeId.ToString()));
                        bike = booking.Bike;
                    }
                    var result = bookings.AsQueryable().PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
                    if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                    var rs = new DynamicModelResponse<BookingViewModel>
                    {
                        Metadata = new PagingMetaData
                        {
                            Page = pageNum,
                            Size = size,
                            Total = result.Item1
                        },
                        Data = result.Item2.ToList()
                    };
                    return rs;
                }
                else if (status == (int)BookingHistoryStatus.Done)
                {
                    var bookings = await Get(b => b.OwnerId.Equals(ownerId)).
                        Where(b => b.Status == (int)BookingStatus.Canceled || b.Status == (int)BookingStatus.Finished)
                        .ProjectTo<BookingViewModel>(_mapper).ToListAsync();
                    if (bookings.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                    foreach (var booking in bookings)
                    {
                        var bike = await _bikeService.GetBikeById(Guid.Parse(booking.BikeId.ToString()));
                        bike = booking.Bike;
                    }
                    var result = bookings.AsQueryable().PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
                    if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                    var rs = new DynamicModelResponse<BookingViewModel>
                    {
                        Metadata = new PagingMetaData
                        {
                            Page = pageNum,
                            Size = size,
                            Total = result.Item1
                        },
                        Data = result.Item2.ToList()
                    };
                    return rs;
                }
                else return null;
            }
            else if (role == (int)RoleConstants.Customer)
            {
                Guid customerId = tokenModel.Id;
                if (status == (int)BookingHistoryStatus.Ongoing)
                {
                    var bookings = await Get(b => b.CustomerId.Equals(customerId)).
                        Where(b => b.Status == (int)BookingStatus.Pending || b.Status == (int)BookingStatus.Inprocess)
                        .ProjectTo<BookingViewModel>(_mapper).ToListAsync();
                    if (bookings.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                    foreach (var booking in bookings)
                    {
                        var bike = await _bikeService.GetBikeById(Guid.Parse(booking.BikeId.ToString()));
                        bike = booking.Bike;
                    }
                    var result = bookings.AsQueryable().PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
                    if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                    var rs = new DynamicModelResponse<BookingViewModel>
                    {
                        Metadata = new PagingMetaData
                        {
                            Page = pageNum,
                            Size = size,
                            Total = result.Item1
                        },
                        Data = result.Item2.ToList()
                    };
                    return rs;
                }
                else if (status == (int)BookingHistoryStatus.Done)
                {
                    var bookings = await Get(b => b.CustomerId.Equals(customerId)).
                        Where(b => b.Status == (int)BookingStatus.Canceled || b.Status == (int)BookingStatus.Finished)
                        .ProjectTo<BookingViewModel>(_mapper).ToListAsync();
                    if (bookings.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                    foreach (var booking in bookings)
                    {
                        var bike = await _bikeService.GetBikeById(Guid.Parse(booking.BikeId.ToString()));
                        bike = booking.Bike;
                    }
                    var result = bookings.AsQueryable().PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
                    if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                    var rs = new DynamicModelResponse<BookingViewModel>
                    {
                        Metadata = new PagingMetaData
                        {
                            Page = pageNum,
                            Size = size,
                            Total = result.Item1
                        },
                        Data = result.Item2.ToList()
                    };
                    return rs;
                }
                else return null;
            }
            else throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature");

        }

        public async Task<List<Booking>> GetByBikeId(Guid id)
        {
            return await Get(b => b.BikeId.Equals(id)).ToListAsync();
        }

        public async Task<BookingViewModel> GetById(Guid id)
        {
            var booking = await Get(b => b.Id.Equals(id)).ProjectTo<BookingViewModel>(_mapper).FirstOrDefaultAsync();
            if (booking == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var bike = await _bikeService.GetBikeById(Guid.Parse(booking.BikeId.ToString()));
            bike = booking.Bike;
            return booking;
        }

        public async Task<BookingDetailViewModel> GetBookingDetailById(Guid id, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            int role = tokenModel.Role;
            if (role == (int)RoleConstants.Customer)
            {
                var booking = await Get(b => b.Id.Equals(id)).ProjectTo<BookingDetailViewModel>(_mapper).FirstOrDefaultAsync();
                if (booking == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                var bike = await _bikeService.GetBikeById(Guid.Parse(booking.BikeId.ToString()));
                booking.Bike = bike;
                var owner = await _utilService.GetOwnerByOwnerId(Guid.Parse(booking.OwnerId.ToString()));
                booking.OwnerName = owner.Fullname;
                booking.PhoneNum = owner.PhoneNumber;
                return booking;
            }
            else if (role == (int)RoleConstants.Owner)
            {
                var booking = await Get(b => b.Id.Equals(id)).ProjectTo<BookingDetailViewModel>(_mapper).FirstOrDefaultAsync();
                if (booking == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                var bike = await _bikeService.GetBikeById(Guid.Parse(booking.BikeId.ToString()));
                booking.Bike = bike;
                var customer = await _customerService.GetCustomerById(Guid.Parse(booking.CustomerId.ToString()));
                booking.CustomerName = customer.Fullname;
                booking.PhoneNum = customer.PhoneNumber;
                return booking;
            }
            else throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature");
        }

        public async Task<List<BookingViewModel>> GetListBookingByOwnerId(Guid id)
        {
            var result = await Get(b => b.OwnerId.Equals(id)).ProjectTo<BookingViewModel>(_mapper).ToListAsync();
            if (result == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            return result;
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

                    var bookingTime = await TrackingBookingTimeUtil.GetBookingTime(targetBooking.Id);

                    var now = DateTime.Now;
                    var dayBook = bookingTime.BookingTime;
                    var dayRent = targetBooking.DayRent;

                    var totalTime = (dayRent - dayBook).Value.TotalHours;

                    var realTime = (dayRent - now).Value.TotalHours;

                    if (realTime / totalTime < 0.3) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Overdue.");

                    targetBike.Status = (int)BikeStatus.Available;

                    isUpdated = true;

                    var check = await NotificationUtil.CancelBooking(userId, role);

                    if (check == false)
                        throw new ErrorResponse((int)HttpStatusCode.InternalServerError, "Something went wrong");

                    decimal advanceMoney = bookingTime.OwnerAdvanceMoney;

                    await _walletService.UpdateAmount(userId, advanceMoney, (int)WalletStatus.DEPOSIT, targetBooking.Id);

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

        public async Task<bool> SaveBookingEvidence(Guid id, string path)
        {
            return await EvidenceUtil.SaveEvidence(id, path);
        }
    }
}

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
        private readonly IVoucherItemService _voucherItemService;
        private readonly IVoucherService _voucherService;
        private readonly IPriceListService _priceListService;
        private readonly ICategoryService _categoryService;
        private readonly IUtilService _utilService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration,
            IBookingRepository bookingRepository,
            IBikeService bikeService,
            IVoucherItemService voucherItemService,
            IVoucherService voucherService,
            IPriceListService priceListService,
            ICategoryService categoryService, 
            IUtilService utilService
            ) : base(unitOfWork, bookingRepository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _bikeService = bikeService;
            _voucherItemService = voucherItemService;
            _voucherService = voucherService;
            _priceListService = priceListService;
            _categoryService = categoryService;
            _utilService = utilService;
        }

        public async Task<BookingSuccessViewModel> CreateNew(string token, BookingCreateRequest request)
        {
            decimal? newPrice = null;
            bool isDiscounted = false;

            Booking targetBooking = _mapper.CreateMapper().Map<Booking>(request);

            List<OwnerByAreaViewModel> owners = await _utilService.GetListOwnerByAreaId(request.AreaId);

            var sortedOwners = from ownerTemp in owners
                               orderby ownerTemp.Rating descending
                               select ownerTemp;

            BikeViewModel suiteableBike = null;

            foreach (var ownerTemp in sortedOwners)
            {
                bool isGotten = false;
                foreach (var bikeTemp in ownerTemp.ListBike)
                {
                    if (bikeTemp.CategoryName.Equals(request.CategoryName) && bikeTemp.Status == (int)BikeStatus.Available)
                    {
                        suiteableBike = bikeTemp;
                        isGotten = true;
                        break;
                    }
                }

                if (isGotten)
                {
                    break;
                }
            }

            if (suiteableBike == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "The suiteable bike with these options is not found.");

            var category = await _categoryService.Get()
                .Where(cateTemp => cateTemp.Name.Equals(request.CategoryName))
                .FirstOrDefaultAsync();

            if (category == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "The category name is not found.");

            decimal price = await _priceListService.GetPriceByAreaIdAndCategoryId(request.AreaId, category.Id);

            targetBooking.Price = price;

            if (price == 0)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot find the price for this area and this category.");

            if (request.VoucherCode != null)
            {
                var voucherItem = await _voucherItemService.GetAsync(request.VoucherCode);

                if (voucherItem == null)
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Voucher code is not existed.");

                if (voucherItem.TimeUsingRemain <= 0)
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Voucher is out of using time.");


                var voucher = await _voucherService.GetAsync(voucherItem.VoucherId);

                if (voucher == null)
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This voucher is not found.");

                if (DateTime.Compare(voucher.StartingDate, DateTime.Today) > 0)
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This voucher is not started yet.");

                if (DateTime.Compare(voucher.ExpiredDate, DateTime.Today) < 0)
                {
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This voucher is expired.");
                }

                newPrice = DiscountBooking(price, voucher.DiscountPercent.Value, voucher.DiscountAmount.Value);
            }

            if (newPrice != null)
            {
                targetBooking.Price = newPrice;
                isDiscounted = true;
            }

            targetBooking.OwnerId = suiteableBike.OwnerId;
            targetBooking.BikeId = suiteableBike.Id;

            if (isDiscounted)
            {
                var voucherItemUsed = await _voucherItemService.GetAsync(request.VoucherCode);
                voucherItemUsed.TimeUsingRemain -= 1;
                await _voucherItemService.UpdateAsync(voucherItemUsed);
            }

            var chosenBike = await _bikeService.GetAsync(suiteableBike.Id);
            chosenBike.Status = (int)BikeStatus.Pending;

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

            targetBooking.Status = request.Status;

            Bike targetBike = await _bikeService.GetAsync(targetBooking.BikeId);
            bool isUpdated = false;

            switch (request.Status)
            {
                case (int)BookingStatus.Pending:
                    throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot update booking status to pending.");

                case (int)BookingStatus.Inprocess:
                    targetBike.Status = (int)BikeStatus.Rent;

                    isUpdated = true;
                    break;

                case (int)BookingStatus.Finished:
                    targetBike.Status = (int)BikeStatus.Available;

                    isUpdated = true;
                    break;

                case (int)BookingStatus.Canceled:
                    targetBike.Status = (int)BikeStatus.Available;

                    isUpdated = true;
                    break;

                default:
                    throw new ErrorResponse((int)HttpStatusCode.NotImplemented, "This status has not implemented yet.");
            }

            if (isUpdated)
            {
                await _bikeService.UpdateAsync(targetBike);
                await UpdateAsync(targetBooking);
            }

            BookingSuccessViewModel resultBooking = _mapper.CreateMapper().Map<BookingSuccessViewModel>(targetBooking);

            return await Task.Run(() => resultBooking);
        }
    }
}

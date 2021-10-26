﻿using AutoMapper;
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
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
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
    public interface IOwnerService : IBaseService<Owner>
    {
        Task<string> Login(OwnerLoginRequest loginRequest, IConfiguration configuration);
        Task<string> Register(OwnerRegisterRequest loginRequest, IConfiguration configuration);
        Task<OwnerViewModel> CreateNew(Owner ownerInfo);
        Task<OwnerViewModel> Delete(Guid id);
        Task<OwnerDetailViewModel> GetOwnerById(Guid id);
        Task<Owner> GetOwner(Guid id);
        Task<OwnerViewModel> GetByMail(string mail);
        Task<DynamicModelResponse<OwnerRatingViewModel>> GetAll(OwnerRatingViewModel model, int filterOption, int size, int pageNum);
        Task<List<OwnerByAreaViewModel>> GetListOwnerByAreaIdAndTypeId(Guid areaId, Guid typeId, string token, DateTime dateRent, DateTime dateReturn, int? timeRent, double totalPrice, string address, string customerLocation);
        Task<bool> SendNoti(Guid ownerId, CustomerRequestModel request);
        //thieu update
    }
    public class OwnerService : BaseService<Owner>, IOwnerService
    {
        private readonly IConfiguration _configuration;
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IBikeService _bikeService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IFeedbackService _feedbackService;
        private readonly IBookingUtilService _bookingUtilService;
        private readonly IBikeUtilService _bikeUtilService;


        public OwnerService(IMapper mapper, IConfiguration configuration, IUnitOfWork unitOfWork,IBikeService bikeService,IFeedbackService feedbackService,
            ICategoryService categoryService, IBrandService brandService, IOwnerRepository repository, 
            IBookingUtilService bookingUtilService
            , IBikeUtilService bikeUtilService) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _bikeService = bikeService;
            _feedbackService = feedbackService;
            _categoryService = categoryService;
            _brandService = brandService;
            _bookingUtilService = bookingUtilService;
            _bikeUtilService = bikeUtilService;
        }

        public async Task<Owner> GetOwner(Guid id)
        {
            var owner = await GetAsync(id);
            return owner;
        }

        public async Task<OwnerDetailViewModel> GetOwnerById(Guid id)
        {
            int total = 0;
            double? rating = 0;
            var owner = Get(x => x.Id.Equals(id)).ProjectTo<OwnerDetailViewModel>(_mapper).FirstOrDefault();
            var listBike = await _bikeService.GetBikeByOwnerId(id);
            if (listBike.Count != 0)
            {
                owner.ListBike = _mapper.CreateMapper().Map<List<BikeViewModel>>(listBike);
                for (int i = 0; i < listBike.Count; i++)
                {
                    var cate = await _categoryService.GetCateById(listBike[i].CategoryId);
                    owner.ListBike[i].CategoryName = cate.Name;
                    var brand = await _brandService.GetBrandById(cate.BrandId);
                    owner.ListBike[i].BrandName = brand.Name;
                    var tempRating = await _feedbackService.GetBikeRating(listBike[i].Id);
                    if (tempRating.FirstOrDefault().Value != 0)
                    {
                        total += tempRating.FirstOrDefault().Key;
                        rating += tempRating.FirstOrDefault().Value;
                    }           
                }
                if (total != 0)
                {
                    owner.Rating = rating / total;
                    owner.NumberOfRatings = total;
                }
                else
                {
                    owner.Rating = 0;
                    owner.NumberOfRatings = total;
                }
            }
            return owner;
        }

        public async Task<OwnerViewModel> GetByMail(string mail)
        {
            return await Get().Where(tempOwner => tempOwner.Mail.Equals(mail)).ProjectTo<OwnerViewModel>(_mapper).FirstOrDefaultAsync();
        }

        public async Task<OwnerViewModel> CreateNew(Owner ownerInfo)
        {
            try
            {
                await CreateAsync(ownerInfo);

                return await GetByMail(ownerInfo.Mail);
            }
            catch
            {
                return null;
            }
        }
        public async Task<DynamicModelResponse<OwnerRatingViewModel>> GetAll(OwnerRatingViewModel model, int filterOption,int size, int pageNum)
        {
            var owners = Get(o => o.Bikes != null).ProjectTo<OwnerRatingViewModel>(_mapper).DynamicFilter<OwnerRatingViewModel>(model);
            List<OwnerRatingViewModel> listOwner = owners.ToList();
            if (listOwner.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            for (int i = 0; i < listOwner.Count; i++)
            {
                double? rating = 0;
                int total = 0;
                var listBike = await _bikeService.GetBikeByOwnerId(Guid.Parse(listOwner[i].Id.ToString()));
                listOwner[i].NumberOfBikes = listBike.Count();
                if(listBike.Count() != 0)
                {
                    foreach (var bike in listBike)
                    {
                        var tempRating = await _feedbackService.GetBikeRating(bike.Id);
                        if(tempRating.FirstOrDefault().Value != 0)
                        {
                            total += tempRating.FirstOrDefault().Key;
                            rating += tempRating.FirstOrDefault().Value;
                        }
                    }
                    if(total != 0)
                    {
                        listOwner[i].Rating = rating / total;
                        listOwner[i].NumberOfRatings = total;
                    }
                    else
                    {
                        listOwner[i].Rating = 0;
                        listOwner[i].NumberOfRatings = total;
                    }
                }
            }
            if (filterOption == (int)OwnerFilterOptions.TotalBike)
            {
                owners = listOwner.AsQueryable().OrderByDescending(o => o.NumberOfBikes);
            }
            else if (filterOption == (int)OwnerFilterOptions.Rating)
            {
                owners = listOwner.AsQueryable().OrderByDescending(o => o.Rating);
            }
            else
            {
                owners = listOwner.AsQueryable().OrderByDescending(o => o.Rating).ThenByDescending(o => o.NumberOfBikes);
            }

            var result = owners.PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

            var rs = new DynamicModelResponse<OwnerRatingViewModel>
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

        public async Task<OwnerViewModel> Delete(Guid id)
        {
            //only admin
            var owner = await GetAsync(id);
            if (owner == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            owner.Status = (int)UserStatus.Deactive;
            owner.BanTimes++;
            await UpdateAsync(owner);
            var result = _mapper.CreateMapper().Map<OwnerViewModel>(owner);
            return result;
        }

        public async Task<string> Login(OwnerLoginRequest loginRequest, IConfiguration configuration)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(loginRequest.GoogleId); // get user by request's guid
            OwnerViewModel result = await GetByMail(userRecord.Email);

            if (result != null) // if email existed in local database
            {
                FirebaseToken token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(loginRequest.AccessToken); // re-check access token with firebase
                object email;
                token.Claims.TryGetValue("email", out email); // get email from the above re-check step, then check the email whether it's matched the request email
                if (userRecord.Email.Equals(email))
                {
                    string verifyRequestToken = TokenService.GenerateOwnerJWTWebToken(result, configuration);

                    TrackingOnlineUtil trackingOnlineUtil = new TrackingOnlineUtil();
                    await trackingOnlineUtil.TrackNewUserLogin(result.Id);

                    return await Task.Run(() => verifyRequestToken); // return if everything is done
                }
                throw new ErrorResponse((int)ResponseStatusConstants.FORBIDDEN, "Email from request and the one from access token is not matched."); // return if this email's not existed yet in database - FE foward to sign up page
            }
            var claim = new Dictionary<string, object> { { "email", userRecord.Email } };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(loginRequest.GoogleId, claim);

            throw new ErrorResponse((int)ResponseStatusConstants.NOT_FOUND, "Email's not existed in database yet.");
        }

        public async Task<string> Register(OwnerRegisterRequest loginRequest, IConfiguration configuration)
        {
            FirebaseToken token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(loginRequest.AccessToken); // get firebase token via request's access token
            object email;
            token.Claims.TryGetValue("email", out email); // get email from token above

            if (loginRequest.Owner.Mail.Equals(email)) // re-check if the email in request and email in the token above is match
            {
                Owner owner = _mapper.CreateMapper().Map<Owner>(loginRequest.Owner);
                OwnerViewModel ownerResult = await CreateNew(owner);

                if (ownerResult != null)
                {
                    string verifyRequestToken =TokenService.GenerateOwnerJWTWebToken(ownerResult, configuration);

                    TrackingOnlineUtil trackingOnlineUtil = new TrackingOnlineUtil();
                    await trackingOnlineUtil.TrackNewUserLogin(ownerResult.Id);

                    return await Task.Run(() => verifyRequestToken);
                }
                throw new ErrorResponse((int)ResponseStatusConstants.FORBIDDEN, "Something went wrong.");
            }
            throw new ErrorResponse((int)ResponseStatusConstants.FORBIDDEN, "Email from request and the one from access token is not matched.");
        }

        public async Task<List<OwnerByAreaViewModel>> GetListOwnerByAreaIdAndTypeId(Guid areaId, Guid typeId, string token, DateTime dateRent, DateTime dateReturn, int? timeRent , double totalPrice, string address, string customerLocation) // customerLocation is formated by "latitude,longitude"
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);
            int role = tokenModel.Role;
            if (role != (int)RoleConstants.Customer)
                throw new ErrorResponse((int)HttpStatusCode.Unauthorized, "This role cannot use this feature.");

            var owners = Get(x => x.AreaId.Equals(areaId)).ProjectTo<OwnerByAreaViewModel>(_mapper);
            var listOwner = owners.ToList();
            if (listOwner.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");

            int flag = 0;
            for (int i = 0; i < listOwner.Count; i++)
            {
                var bike = await _bikeUtilService.FindBike(listOwner[i].Id, typeId);

                if (bike == null)
                {
                    flag++;
                }
                else
                {
                    listOwner[i].Bike = bike;
                    listOwner[i].Rating = bike.Rating;
                }
            }

            if (flag == listOwner.Count)
                throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");

            var result = listOwner.AsQueryable().OrderByDescending(o => o.Rating);
            var listTemp = result.ToList();

            for (int i = 0; i < listTemp.Count; i++)
            {
                TrackingOnlineUtil trackingOnlineUtil = new TrackingOnlineUtil();
                DateTime? expiredTime = await trackingOnlineUtil.GetUserExpiredTime(listTemp[i].Id);

                if (expiredTime == null || TrackingOnlineUtil.IsExpired(expiredTime) || listTemp[i].Bike == null)
                {
                    listTemp.RemoveAt(i);
                    i--;
                }
            }
            var listDistance = await DistanceUtil.OrderByDistance(listTemp, customerLocation);
            foreach(var ownerTemp in listDistance)
            {
                double bookingtimes = 0;
                double deniedtimes = 0;
                var trackingBooking = await TrackingBookingUtil.GetTrackingBookingByDate(ownerTemp.Id, dateRent);
                if(trackingBooking != null)
                {
                    bookingtimes = double.Parse(trackingBooking.BookingTimes.ToString());
                    deniedtimes = double.Parse(trackingBooking.DeniedTimes.ToString());
                }              
                double totalBike = double.Parse(ownerTemp.Bike.TotalBike.ToString());
                double distance = double.Parse(ownerTemp.LocationInfo.Distance.ToString());
                double rating = double.Parse(ownerTemp.Rating.ToString());
                ownerTemp.PriorityPoint = PriorityUtil.GetPriority(bookingtimes, totalBike, distance, rating, deniedtimes);
            }
            var finalResult = listDistance.AsQueryable().OrderByDescending(rs => rs.PriorityPoint);
            var rs = finalResult.ToList();
            CustomerRequestModel request = new CustomerRequestModel();
            request.LicensePlate = rs[0].Bike.LicensePlate;
            request.CateName = rs[0].Bike.CateName;
            request.ImgPath = rs[0].Bike.ImgPath;
            request.DateRent = dateRent;
            request.DateReturn = dateReturn;
            request.Price = totalPrice;
            request.Address = address;
            request.CustomerName = tokenModel.Name;
            var trackingRegistrationId = await TrackingRegistrationIdUtil.GetOwnerRegistrationId(rs[0].Id);
            var registrationId = trackingRegistrationId.RegistrationId;
            var checkSendNoti = await NotificationUtil.SendNotification(registrationId, request);
            if (checkSendNoti == false) throw new ErrorResponse((int)HttpStatusCode.InternalServerError, "Something went wrong.");
            return rs;
        }

        public async Task<bool> SendNoti(Guid ownerId, CustomerRequestModel request)
        {
            var trackingRegistrationId = await TrackingRegistrationIdUtil.GetOwnerRegistrationId(ownerId);
            var registrationId = trackingRegistrationId.RegistrationId;
            var send = await NotificationUtil.SendNotification(registrationId, request);
            if (send == false) throw new ErrorResponse((int)HttpStatusCode.InternalServerError, "Something went wrong.");
            return send;
        }
    }
}

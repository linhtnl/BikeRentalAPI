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
        Task<DynamicModelResponse<OwnerRatingViewModel>> GetAll(OwnerRatingViewModel model, int filterOption, int pageNum);
        Task<List<OwnerByAreaViewModel>> GetListOwnerByAreaId(Guid id);
    }
    public class OwnerService : BaseService<Owner>, IOwnerService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IBikeService _bikeService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IFeedbackService _feedbackService;
        public OwnerService(IUnitOfWork unitOfWork,IBikeService bikeService,IFeedbackService feedbackService,
            ICategoryService categoryService, IBrandService brandService, IOwnerRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _bikeService = bikeService;
            _feedbackService = feedbackService;
            _categoryService = categoryService;
            _brandService = brandService;
            _mapper = mapper.ConfigurationProvider;
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
        public async Task<DynamicModelResponse<OwnerRatingViewModel>> GetAll(OwnerRatingViewModel model, int filterOption, int pageNum)
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

            var result = owners.PagingIQueryable(pageNum, GlobalConstants.GROUP_ITEM_NUM, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

            var rs = new DynamicModelResponse<OwnerRatingViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = pageNum,
                    Size = GlobalConstants.GROUP_ITEM_NUM,
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
            if (owner == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Can not found");
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
                    string verifyRequestToken = new TokenService(configuration).GenerateOwnerJWTWebToken(result);

                    return await Task.Run(() => verifyRequestToken); // return if everything is done
                }
                throw new ErrorResponse((int)ResponseStatusConstants.FORBIDDEN, "Email from request and the one from access token is not matched."); // return if this email's not existed yet in database - FE foward to sign up page
            }
            var claim = new Dictionary<string, object> { { "email", userRecord.Email } };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(loginRequest.GoogleId, claim);

            throw new ErrorResponse((int)ResponseStatusConstants.CREATED, "Email's not existed in database yet.");
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
                    string verifyRequestToken = new TokenService(configuration).GenerateOwnerJWTWebToken(ownerResult);

                    return await Task.Run(() => verifyRequestToken);
                }
                throw new ErrorResponse((int)ResponseStatusConstants.FORBIDDEN, "Something went wrong.");
            }
            throw new ErrorResponse((int)ResponseStatusConstants.FORBIDDEN, "Email from request and the one from access token is not matched.");
        }

        public async Task<List<OwnerByAreaViewModel>> GetListOwnerByAreaId(Guid id)
        {
            int total = 0;
            double? rating = 0;
            var owners = Get(x => x.AreaId.Equals(id)).ProjectTo<OwnerByAreaViewModel>(_mapper);
            var listOwner = owners.ToList();
            if(listOwner.Count==0) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Can not found");
            for (int i = 0; i < listOwner.Count;i++)
            {
                var listBike = await _bikeService.GetBikeByOwnerId(listOwner[i].Id);
                if (listBike.Count != 0)
                {
                    listOwner[i].ListBike = _mapper.CreateMapper().Map<List<BikeViewModel>>(listBike);
                    for (int j = 0; j < listBike.Count; j++)
                    {
                        var cate = await _categoryService.GetCateById(listBike[j].CategoryId);
                        listOwner[i].ListBike[j].CategoryName = cate.Name;
                        var brand = await _brandService.GetBrandById(cate.BrandId);
                        listOwner[i].ListBike[j].BrandName = brand.Name;
                        var tempRating = await _feedbackService.GetBikeRating(listBike[j].Id);
                        if (tempRating.FirstOrDefault().Value != 0)
                        {
                            total += tempRating.FirstOrDefault().Key;
                            rating += tempRating.FirstOrDefault().Value;
                        }
                    }
                    if (total != 0)
                    {
                        listOwner[i].Rating = rating / total;
                    }
                    else
                    {
                        listOwner[i].Rating = 0;
                    }
                }
            }
            var result = listOwner.ToList();
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].ListBike == null)
                {
                    result.RemoveAt(i);
                    i--;
                }
            }
            if (result[0].ListBike == null)
            {
                result.RemoveAt(0);
            }
            return result;
        }
    }
}

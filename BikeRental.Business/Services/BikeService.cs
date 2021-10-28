using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using BikeRental.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using BikeRental.Business.Utilities;
using Microsoft.EntityFrameworkCore;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BikeRental.Business.Services
{
    public interface IBikeService : IBaseService<Bike>
    {
        Task<DynamicModelResponse<BikeViewModel>> GetAll(BikeViewModel model,int size, int pageNum, string token);
        Task<BikeByIdViewModel> GetBikeById(Guid id);
        Task<BikeViewModel> Create(BikeCreateRequest request, string token);
        Task<BikeViewModel> Update(BikeUpdateRequest request, string token);
        Task<BikeDeleteSuccessViewModel> Delete(Guid id, string token);
        Task<List<Bike>> GetBikeByOwnerId(Guid id);
    }
    public class BikeService : BaseService<Bike>, IBikeService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IWalletUtilService _walletUtilService;


        public BikeService(IUnitOfWork unitOfWork, IBikeRepository repository,
             ICategoryService categoryService,IBrandService brandService, IWalletUtilService walletUtilService, IMapper mapper,IConfiguration configuration) : base(unitOfWork, repository)
        {
            _categoryService = categoryService;
            _brandService = brandService;
            _walletUtilService = walletUtilService;
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;
        }

        public async Task<BikeByIdViewModel> GetBikeById(Guid id)
        {
            var bike = await Get(x => x.Id.Equals(id)).ProjectTo<BikeByIdViewModel>(_mapper).FirstOrDefaultAsync();
            var cate = await _categoryService.GetCateById(bike.CategoryId);
            bike.BrandId = cate.BrandId;
            return bike;
        }

        public async Task<DynamicModelResponse<BikeViewModel>> GetAll(BikeViewModel model, int size, int pageNum, string token)
        {
            //if user = admin BikeStatus : show all
            //if user = owner BikeStatus : Available, Rent base on ownerID

            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);
            int role = tokenModel.Role;
            if (role == (int)RoleConstants.Admin)
            {
                var bikes = Get().ProjectTo<BikeViewModel>(_mapper);
                List<BikeViewModel> listBike = bikes.ToList();
                if (listBike.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
                for (int i = 0; i < listBike.Count; i++)
                {
                    var cate = await _categoryService.GetCateById(listBike[i].CategoryId);
                    var brand = await _brandService.GetBrandById(cate.BrandId);
                    listBike[i].BrandName = brand.Name;
                }
                bikes = listBike.AsQueryable().OrderByDescending(b => b.BrandName).ThenByDescending(b => b.CategoryName);
                var result = bikes
                    .DynamicFilter(model)
                    .PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
                if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                var rs = new DynamicModelResponse<BikeViewModel>
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
            }else if(role == (int)RoleConstants.Owner)
            {
                var bikes = Get(x => x.Status == (int)BikeStatus.Available).ProjectTo<BikeViewModel>(_mapper);
                List<BikeViewModel> listBike = bikes.ToList();
                if (listBike.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
                for (int i = 0; i < listBike.Count; i++)
                {
                    var cate = await _categoryService.GetCateById(listBike[i].CategoryId);
                    var brand = await _brandService.GetBrandById(cate.BrandId);
                    listBike[i].BrandName = brand.Name;
                }
                bikes = listBike.AsQueryable().OrderByDescending(b => b.BrandName).ThenByDescending(b => b.CategoryName);
                var result = bikes
                    .DynamicFilter(model)
                    .PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
                if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
                var rs = new DynamicModelResponse<BikeViewModel>
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
            else
            {
                throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");
            }

            
        }

        public async Task<BikeViewModel> Create(BikeCreateRequest request, string token)
        {
            //get id from token

            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);
            int role = tokenModel.Role;
            if (role == (int)RoleConstants.Owner)
            {
                
                var bike = _mapper.CreateMapper().Map<Bike>(request);
                var balance = await _walletUtilService.GetWalletBalance(bike.OwnerId.Value);
                if (decimal.Compare((decimal)balance,50000)<0) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Your Balance is not enough to create new bike.");
                var cate = await _categoryService.GetCateById(bike.CategoryId);
                if (cate.Status == (int)CategoryStatus.Pending)
                {
                    bike.Status = (int)BikeStatus.Pending;
                }
                else
                {
                    bike.Status = (int)BikeStatus.Available;
                }
                try
                {
                    await CreateAsync(bike);
                    var result = _mapper.CreateMapper().Map<BikeViewModel>(bike);
                    result.CategoryName = cate.Name;
                    var brand = await _brandService.GetBrandById(cate.BrandId);
                    result.BrandName = brand.Name;
                    return result;
                }
                catch (Exception e)
                {
                    if (e.InnerException.ToString().Contains("UNIQUE"))
                    {
                        /*bike.Status = (int)BikeStatus.Pending;
                        bike.LicensePlate = "[" + bike.LicensePlate + "]";
                        await CreateAsync(bike);
                        var result = _mapper.CreateMapper().Map<BikeViewModel>(bike);
                        result.CategoryName = cate.Name;
                        var brand = await _brandService.GetBrandById(cate.BrandId);
                        result.BrandName = brand.Name;
                        return result;*/
                        //send noti to admin
                        throw new ErrorResponse((int)HttpStatusCode.BadRequest, "License Plate is exist.");
                    }
                    throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "Invalid data.");
                }
            }
            else
            {
                throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");
            }

                
        }

        public async Task<BikeViewModel> Update(BikeUpdateRequest request, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);
            int role = tokenModel.Role;
            if (role == (int)RoleConstants.Owner||role == (int)RoleConstants.Admin)
            {
                var bike = await GetAsync(request.Id);
                if (bike == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
                if(tokenModel.Role == (int)RoleConstants.Owner && !bike.OwnerId.Equals(tokenModel.Id)) throw new ErrorResponse((int)HttpStatusCode.Forbidden, "You can only update information of your bike.");
                var updateBike = _mapper.CreateMapper().Map(request, bike);
                try
                {
                    await UpdateAsync(updateBike);
                    var result = _mapper.CreateMapper().Map<BikeViewModel>(updateBike);
                    var cate = await _categoryService.GetCateById(bike.CategoryId);
                    result.CategoryName = cate.Name;
                    var brand = await _brandService.GetBrandById(cate.BrandId);
                    result.BrandName = brand.Name;
                    return result;
                }
                catch (Exception e)
                {
                    if (e.InnerException.ToString().Contains("UNIQUE"))
                    {
                        /*bike.Status = (int)BikeStatus.Pending;
                        bike.LicensePlate = "[" + bike.LicensePlate + "]";
                        await CreateAsync(bike);
                        var result = _mapper.CreateMapper().Map<BikeViewModel>(bike);
                        result.CategoryName = cate.Name;
                        var brand = await _brandService.GetBrandById(cate.BrandId);
                        result.BrandName = brand.Name;
                        return result;*/
                        //send noti to admin
                        throw new ErrorResponse((int)HttpStatusCode.BadRequest, "License Plate is exist.");
                    }
                    throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "Invalid data!");
                }
            }
            else
            {
                throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");
            }
                
            
        }

        public async Task<BikeDeleteSuccessViewModel> Delete(Guid id, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);
            int role = tokenModel.Role;
            if (role == (int)RoleConstants.Owner || role == (int)RoleConstants.Admin)
            {
                var bike = await GetAsync(id);

                if (bike == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
                if (bike.Status == (int)BikeStatus.Rent) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Can not delete Rented bike!");
                bike.LicensePlate = bike.LicensePlate + "_Delete_" + DateTime.Now;
                bike.Status = (int)BikeStatus.Delete;
                await UpdateAsync(bike);
                var result = _mapper.CreateMapper().Map<BikeDeleteSuccessViewModel>(bike);
                return result;
            }
            else
            {
                throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");
            }

        }

        public async Task<List<Bike>> GetBikeByOwnerId(Guid id)
        {
            return await Get(b => b.OwnerId.Equals(id)).ToListAsync();
        }

        
    }
}

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
    public interface IVoucherService : IBaseService<Voucher>
    {
        Task<Voucher> CreateNew(VoucherCreateRequest voucherRequest);
        Task<Voucher> UpdateVoucher(Guid id, VoucherUpdateRequest voucherRequest);
        Task<Voucher> DeleteVoucher(Guid id);
        List<VoucherViewModel> GetAll();
        Task<VoucherViewModel> GetById(Guid id);
        Task<List<VoucherViewModel>> GetByCampaignId(Guid campaignId);
        Task<List<VoucherViewModel>>  GetByAreaId(Guid areaId, string token);
        List<VoucherViewModel> GetInDiscountPercentRange(int startNum, int endNum);
        List<VoucherViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate); // this method is used to get all campaign that start in the range time
        List<VoucherViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate); // this method is used to get all campaign that end in the range time
        Task<decimal> GetDiscountedPrice(decimal originalPrice, Guid voucherCode);
    }
    public class VoucherService : BaseService<Voucher>, IVoucherService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;

        private readonly IVoucherItemRepository _voucherItemRepository;
        private readonly ICampaignService _campaignService;
        private readonly ICustomerRepository _customerRepository;
        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, 
            IVoucherRepository voucherRepository, 
            IVoucherItemRepository voucherItemRepository,
            ICampaignService campaignService, 
            ICustomerRepository customerRepository
            ) : base(unitOfWork, voucherRepository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _voucherItemRepository = voucherItemRepository;
            _campaignService = campaignService;
            _customerRepository = customerRepository;
        }

        public async Task<Voucher> CreateNew(VoucherCreateRequest voucherRequest)
        {
            if (DateTime.Compare(voucherRequest.StartingDate, voucherRequest.ExpiredDate) >= 0)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Starting date must earlier than expired date.");

            if (DateTime.Compare(voucherRequest.ExpiredDate, DateTime.Today) < 0)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Expired date must same or later than today.");

            if (DateTime.Compare(voucherRequest.StartingDate, DateTime.Today) < 0)
                throw new ErrorResponse((int) HttpStatusCode.Forbidden, "Starting date must same or later than today.");

            var voucher = (_mapper).CreateMapper().Map<Voucher>(voucherRequest);
            await CreateAsync(voucher);

            return await Task.Run(() => voucher);
        }

        public List<VoucherViewModel> GetAll()
        {
            return Get()
                .ProjectTo<VoucherViewModel>(_mapper)
                .ToList();
        }

        public async Task<List<VoucherViewModel>> GetByCampaignId(Guid campaignId)
        {
            return await Get().Where(tempVoucher => (tempVoucher.CampaignId.Equals(campaignId) && tempVoucher.VoucherItemsRemain > 0))
                .ProjectTo<VoucherViewModel>(_mapper)
                .ToListAsync();
        }

        public async Task<VoucherViewModel> GetById(Guid id)
        {
            var result = await Get().Where(tempVoucher => tempVoucher.Id.Equals(id))
                .ProjectTo<VoucherViewModel>(_mapper)
                .FirstOrDefaultAsync();

            return await Task.Run(() => result);
        }

        public List<VoucherViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate)
        {
            return Get().Where(tempVoucher => tempVoucher.StartingDate >= startDate && tempVoucher.StartingDate <= endDate)
                .ProjectTo<VoucherViewModel>(_mapper)
                .ToList();
        }

        public List<VoucherViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate)
        {
            return Get().Where(tempVoucher => tempVoucher.ExpiredDate >= startDate && tempVoucher.StartingDate <= endDate)
                .ProjectTo<VoucherViewModel>(_mapper)
                .ToList();
        }

        public List<VoucherViewModel> GetInDiscountPercentRange(int startNum, int endNum)
        {
            return Get().Where(tempVoucher => tempVoucher.DiscountPercent >= startNum && tempVoucher.DiscountPercent <= endNum)
                .ProjectTo<VoucherViewModel>(_mapper)
                .ToList();
        }

        public async Task<Voucher> UpdateVoucher(Guid id, VoucherUpdateRequest voucherRequest)
        {
            var voucher = await GetAsync(id);
            if (voucher == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Voucher not found");
            
            var targetVoucher = _mapper.CreateMapper().Map<Voucher>(voucherRequest);
            await UpdateAsync(targetVoucher);

            return targetVoucher;
        }

        public async Task<Voucher> DeleteVoucher(Guid id)
        {
            var voucher = await GetAsync(id);
            if (voucher == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Voucher not found");

            voucher.Status = (int)VoucherStatus.Delete;
            
            await UpdateAsync(voucher);

            return await Task.Run(() => voucher);
        }

        public async Task<decimal> GetDiscountedPrice(decimal originalPrice, Guid voucherCode)
        {
            var voucherItem = await _voucherItemRepository.GetAsync(voucherCode);

            if (voucherItem == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This voucher code is not existed.");

            var voucher = await GetById(voucherItem.VoucherId.Value);

            int discountPercent = voucher.DiscountPercent.Value;
            decimal maxDiscountAmount = voucher.DiscountAmount.Value;

            var finalPrice = DiscountUtil.DiscountBooking(originalPrice, discountPercent, maxDiscountAmount);

            return await Task.Run(() => finalPrice.Value);
        }

        public async Task<List<VoucherViewModel>> GetByAreaId(Guid areaId, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            int role = tokenModel.Role;
            if (role != (int)RoleConstants.Customer)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");

            var customerId = tokenModel.Id;

            var customer = await _customerRepository.GetAsync(customerId);
            var currentPoint = customer.RewardPoints;

            var campaigns = await _campaignService.GetByAreaId(areaId);

            List<VoucherViewModel> resultList = new();

            foreach (var campaignTemp in campaigns)
            {
                var vouchers = await GetByCampaignId(campaignTemp.Id.Value);

                if (resultList.Count <= 0)
                {
                    resultList = vouchers;
                    continue;
                }

                resultList.AddRange(vouchers);
            }
            
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i].PointExchange > currentPoint || DateTime.Compare(resultList[i].ExpiredDate, DateTime.Now) <= 0)
                {
                    resultList.RemoveAt(i);
                    i--;
                }
            }

            return await Task.Run(() => resultList);
        }
    }
}

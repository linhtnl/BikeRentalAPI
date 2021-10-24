using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Utilities;
using BikeRental.Data.Enums;
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
    public interface IVoucherService : IBaseService<Voucher>
    {
        Task<Voucher> CreateNew(VoucherCreateRequest voucherRequest);
        Task<Voucher> UpdateVoucher(Guid id, VoucherUpdateRequest voucherRequest);
        Task<Voucher> DeleteVoucher(Guid id);
        List<VoucherViewModel> GetAll();
        Task<VoucherViewModel> GetById(Guid id);
        List<VoucherViewModel> GetByCampaignId(Guid campaignId);
        List<VoucherViewModel> GetInDiscountPercentRange(int startNum, int endNum);
        List<VoucherViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate); // this method is used to get all campaign that start in the range time
        List<VoucherViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate); // this method is used to get all campaign that end in the range time
        Task<decimal> GetDiscountedPrice(decimal originalPrice, Guid voucherCode);
    }
    public class VoucherService : BaseService<Voucher>, IVoucherService
    {
        private readonly IConfigurationProvider _mapper;

        private readonly IVoucherItemRepository _voucherItemRepository;
        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper, 
            IVoucherRepository voucherRepository, 
            IVoucherItemRepository voucherItemRepository
            ) : base(unitOfWork, voucherRepository)
        {
            _mapper = mapper.ConfigurationProvider;

            _voucherItemRepository = voucherItemRepository;
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

        public List<VoucherViewModel> GetByCampaignId(Guid campaignId)
        {
            return Get().Where(tempVoucher => tempVoucher.CampaignId.Equals(campaignId))
                .ProjectTo<VoucherViewModel>(_mapper)
                .ToList();
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
    }
}

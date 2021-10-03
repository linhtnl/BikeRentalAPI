using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IVoucherService : IBaseService<Voucher>
    {
        Task<bool> CreateNew(VoucherViewModel voucherRequest);
        List<VoucherViewModel> GetAll();
        VoucherViewModel GetById(Guid id);
        List<VoucherViewModel> GetByCampaignId(Guid campaignId);
        List<VoucherViewModel> GetInDiscountPercentRange(int startNum, int endNum);
        List<VoucherViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate); // this method is used to get all campaign that start in the range time
        List<VoucherViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate); // this method is used to get all campaign that end in the range time
    }
    public class VoucherService : BaseService<Voucher>, IVoucherService
    {
        private readonly IConfigurationProvider _mapper;
        public VoucherService(IUnitOfWork unitOfWork, IVoucherRepository voucherRepository, IMapper mapper) : base(unitOfWork, voucherRepository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<bool> CreateNew(VoucherViewModel voucherRequest)
        {
            try
            {
                var voucher = (_mapper).CreateMapper().Map<Voucher>(voucherRequest);
                await CreateAsync(voucher);

                return true;
            }
            catch
            {
                return false;
            }
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

        public VoucherViewModel GetById(Guid id)
        {
            return Get().Where(tempVoucher => tempVoucher.Id.Equals(id))
                .ProjectTo<VoucherViewModel>(_mapper)
                .FirstOrDefault();
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
    }
}

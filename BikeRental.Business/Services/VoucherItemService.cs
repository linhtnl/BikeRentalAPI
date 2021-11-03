using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.RequestModels;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IVoucherItemService : IBaseService<VoucherItem>
    {
        Task<VoucherItemCreateSuccessViewModel> CreateNew(VoucherItemCreateRequest voucherItemRequest);
        Task<VoucherItem> UpdateVoucherItem(Guid id, VoucherItemUpdateRequest voucherItemRequest);
        List<VoucherItemViewModel> GetAll();
        VoucherItemViewModel GetById(Guid id);
        Task<List<VoucherItemViewModel>> GetByCustomerId(Guid customerId);
        List<VoucherItemViewModel> GetByVoucherId(Guid voucherId);
    }
    public class VoucherItemService : BaseService<VoucherItem>, IVoucherItemService
    {
        private readonly IConfigurationProvider _mapper;

        private readonly IVoucherExchangeHistoryService _voucherExchangeService;
        private readonly IVoucherService _voucherService;
        private readonly ICustomerService _customerService;

        public VoucherItemService(IUnitOfWork unitOfWork, IVoucherItemRepository voucherItemRepository, IMapper mapper, 
            IVoucherExchangeHistoryService voucherExchangeService, 
            IVoucherService voucherService, 
            ICustomerService customerService
            ) : base(unitOfWork, voucherItemRepository)
        {
            _mapper = mapper.ConfigurationProvider;
            _voucherExchangeService = voucherExchangeService;
            _voucherService = voucherService;
            _customerService = customerService;
        }

        public async Task<VoucherItemCreateSuccessViewModel> CreateNew(VoucherItemCreateRequest voucherItemRequest)
        {
            var voucher = await _voucherService.GetAsync(voucherItemRequest.VoucherId);
            var customer = await _customerService.GetAsync(voucherItemRequest.CustomerId);

            if (customer.RewardPoints < voucher.PointExchange)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Customer's point is not enough to exchange this voucher.");

            var voucherItem = _mapper.CreateMapper().Map<VoucherItem>(voucherItemRequest);
            voucherItem.TimeUsingRemain = voucher.NumberOfUses;
            await CreateAsync(voucherItem);

            await _voucherExchangeService.CreateNew(voucherItem.Id, voucherItem.CustomerId.Value);

            var exchangeHistory = await _voucherExchangeService.GetAsync(voucherItem.Id);

            voucher.VoucherItemsRemain -= 1;

            await _voucherService.UpdateAsync(voucher);

            var voucherItemResult = _mapper.CreateMapper().Map<VoucherItemCreateSuccessViewModel>(voucherItemRequest);
            voucherItemResult.Id = voucherItem.Id;
            voucherItemResult.TimeUsingRemain = voucherItem.TimeUsingRemain;
            voucherItemResult.PointExchange = voucher.PointExchange;
            voucherItemResult.voucherExchange = _mapper.CreateMapper().Map<VoucherExchangeHistoryViewModel>(exchangeHistory);

            customer.RewardPoints -= voucher.PointExchange;
            await _customerService.UpdateAsync(customer);

            return await Task.Run(() => voucherItemResult);
        }

        public List<VoucherItemViewModel> GetAll()
        {
            return Get().ProjectTo<VoucherItemViewModel>(_mapper).ToList();
        }

        public async Task<List<VoucherItemViewModel>> GetByCustomerId(Guid customerId)
        {
            List<VoucherItemViewModel> voucherItems = Get()
                .Where(tempVoucherItem => (tempVoucherItem.CustomerId.Equals(customerId) && tempVoucherItem.TimeUsingRemain > 0))
                .ProjectTo<VoucherItemViewModel>(_mapper)
                .ToList();

            for (int i = 0; i < voucherItems.Count; i++)
            {
                var voucherTemp = await _voucherService.GetById(voucherItems[i].VoucherId.Value);
                voucherItems[i].Voucher = voucherTemp;
                if (!IsVoucherItemInUse(voucherTemp.StartingDate, voucherTemp.ExpiredDate)
                    || !IsVoucherItemRemainUsingTime(voucherItems[i]))
                {
                    voucherItems.RemoveAt(i);
                    i--;
                } else
                {
                    voucherItems[i].ExpiredDate = voucherTemp.ExpiredDate;
                }
            }

            return await Task.Run(() => voucherItems);
        }

        private static bool IsVoucherItemInUse(DateTime startingDate, DateTime expireDate)
        {
            if (DateTime.Compare(startingDate, DateTime.Now) > 0 || DateTime.Compare(expireDate, DateTime.Now) <= 0)
                return false;

            return true;
        }

        private static bool IsVoucherItemRemainUsingTime(VoucherItemViewModel voucherItem)
        {
            if (voucherItem.TimeUsingRemain <= 0)
                return false;

            return true;
        }

        public VoucherItemViewModel GetById(Guid id)
        {
            return Get().Where(tempVoucherItem => tempVoucherItem.Id.Equals(id))
                .ProjectTo<VoucherItemViewModel>(_mapper)
                .FirstOrDefault();
        }

        public List<VoucherItemViewModel> GetByVoucherId(Guid voucherId)
        {
            return Get().Where(tempVoucherItem => tempVoucherItem.VoucherId.Equals(voucherId))
                .ProjectTo<VoucherItemViewModel>(_mapper)
                .ToList();
        }

        public async Task<VoucherItem> UpdateVoucherItem(Guid id, VoucherItemUpdateRequest voucherItemRequest)
        {
            var voucherItem = await GetAsync(id);
            if (voucherItem == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Voucher Item not found");

            var targetVoucherItem = _mapper.CreateMapper().Map<VoucherItem>(voucherItem);
            await UpdateAsync(targetVoucherItem);

            return targetVoucherItem;
        }
    }
}

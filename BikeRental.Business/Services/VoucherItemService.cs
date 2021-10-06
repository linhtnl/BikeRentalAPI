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
        Task<VoucherItem> CreateNew(VoucherItemCreateRequest voucherItemRequest);
        Task<VoucherItem> UpdateVoucherItem(Guid id, VoucherItemUpdateRequest voucherItemRequest);
        //Task<VoucherItem> DeleteVoucherItem(Guid id);
        List<VoucherItemViewModel> GetAll();
        VoucherItemViewModel GetById(Guid id);
        List<VoucherItemViewModel> GetByCustomerId(Guid customerId);
        List<VoucherItemViewModel> GetByVoucherId(Guid voucherId);
    }
    public class VoucherItemService : BaseService<VoucherItem>, IVoucherItemService
    {
        private readonly IConfigurationProvider _mapper;
        public VoucherItemService(IUnitOfWork unitOfWork, IVoucherItemRepository voucherItemRepository, IMapper mapper) : base(unitOfWork, voucherItemRepository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<VoucherItem> CreateNew(VoucherItemCreateRequest voucherItemRequest)
        {
            try
            {
                var voucherItem = _mapper.CreateMapper().Map<VoucherItem>(voucherItemRequest);
                await CreateAsync(voucherItem);

                return voucherItem;
            }
            catch
            {
                return null;
            }
        }

        //public async Task<VoucherItem> DeleteVoucherItem(Guid id)
        //{
        //    var voucherItem = await GetAsync(id);
        //    if (voucherItem == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Voucher Item not found");

        //    voucherItem.s
        //}

        public List<VoucherItemViewModel> GetAll()
        {
            return Get().ProjectTo<VoucherItemViewModel>(_mapper).ToList();
        }

        public List<VoucherItemViewModel> GetByCustomerId(Guid customerId)
        {
            return Get().Where(tempVoucherItem => tempVoucherItem.CustomerId.Equals(customerId))
                .ProjectTo<VoucherItemViewModel>(_mapper)
                .ToList();
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
            if (voucherItem == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Voucher Item not found");

            var targetVoucherItem = _mapper.CreateMapper().Map<VoucherItem>(voucherItem);
            await UpdateAsync(targetVoucherItem);

            return targetVoucherItem;
        }
    }
}

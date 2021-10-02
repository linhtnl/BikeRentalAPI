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
    public interface IVoucherItemService : IBaseService<VoucherItem>
    {
        Task<bool> CreateNew(VoucherItemViewModel voucherItemRequest);
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

        public async Task<bool> CreateNew(VoucherItemViewModel voucherItemRequest)
        {
            try
            {
                VoucherItem voucherItem = new VoucherItem(voucherItemRequest);
                await CreateAsync(voucherItem);
                return true;
            }
            catch
            {
                return false;
            }
        }

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
    }
}

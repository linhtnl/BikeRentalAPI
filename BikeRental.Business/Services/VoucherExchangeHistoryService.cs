using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.RequestModels;
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
    public interface IVoucherExchangeHistoryService : IBaseService<VoucherExchangeHistory>
    {
        Task<VoucherExchangeHistory> CreateNew(Guid voucherId, VoucherExchangeHistoryCreateRequest request);
        Task<VoucherExchangeHistory> UpdateVoucherExchangeHistory(Guid id, VoucherExchangeHistoryUpdateRequest request);
        Task<VoucherExchangeHistoryViewModel> GetById(Guid id);
    }
    public class VoucherExchangeHistoryService : BaseService<VoucherExchangeHistory>, IVoucherExchangeHistoryService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly IVoucherItemService _voucherItemService;
        public VoucherExchangeHistoryService(IUnitOfWork unitOfWork, IVoucherExchangeHistoryRepository voucherExchangeHistoryRepository, IVoucherItemService voucherItemService, IMapper mapper) 
            : base(unitOfWork, voucherExchangeHistoryRepository)
        {
            _mapper = mapper.ConfigurationProvider;
            _voucherItemService = voucherItemService;
        }

        public async Task<VoucherExchangeHistory> CreateNew(Guid voucherId, VoucherExchangeHistoryCreateRequest request)
        {
            return null;
        }

        public async Task<VoucherExchangeHistoryViewModel> GetById(Guid id)
        {
            return await Get().Where(tempVoucherExchangeHistory => tempVoucherExchangeHistory.Id.Equals(id))
                .ProjectTo<VoucherExchangeHistoryViewModel>(_mapper)
                .FirstOrDefaultAsync();
        }

        public async Task<VoucherExchangeHistory> UpdateVoucherExchangeHistory(Guid id, VoucherExchangeHistoryUpdateRequest request)
        {
            var tempVoucherExchangeHistory = await GetById(id);
            if (tempVoucherExchangeHistory == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Voucher Exchange History not found.");

            var targetVoucherExchangeHistory = _mapper.CreateMapper().Map<VoucherExchangeHistory>(request);
            await UpdateAsync(targetVoucherExchangeHistory);
            return targetVoucherExchangeHistory;
        }
    }
}

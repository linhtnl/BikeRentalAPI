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
        Task<VoucherExchangeHistory> CreateNew(Guid voucherId, Guid customerId);
        Task<VoucherExchangeHistory> UpdateVoucherExchangeHistory(Guid id, VoucherExchangeHistoryUpdateRequest request);
        Task<VoucherExchangeHistoryViewModel> GetById(Guid id);
    }
    public class VoucherExchangeHistoryService : BaseService<VoucherExchangeHistory>, IVoucherExchangeHistoryService
    {
        private readonly IConfigurationProvider _mapper;
        public VoucherExchangeHistoryService(IUnitOfWork unitOfWork, IVoucherExchangeHistoryRepository voucherExchangeHistoryRepository, IMapper mapper) 
            : base(unitOfWork, voucherExchangeHistoryRepository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<VoucherExchangeHistory> CreateNew(Guid voucherId, Guid customerId)
        {
            VoucherExchangeHistory targetVoucherExchange = new VoucherExchangeHistory()
            {
                Id = voucherId,
                VoucherCode = voucherId,
                CustomerId = customerId,
                ActionDate = DateTime.Today
            };

            await CreateAsync(targetVoucherExchange);

            return await Task.Run(() => targetVoucherExchange);
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
            if (tempVoucherExchangeHistory == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Voucher Exchange History not found.");

            var targetVoucherExchangeHistory = _mapper.CreateMapper().Map<VoucherExchangeHistory>(request);
            await UpdateAsync(targetVoucherExchangeHistory);
            return targetVoucherExchangeHistory;
        }
    }
}

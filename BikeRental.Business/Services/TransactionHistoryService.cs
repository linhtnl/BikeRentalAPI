using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
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
    public interface ITransactionHistoryService : IBaseService<TransactionHistory>
    {
        TransactionHistoryViewModel GetById(Guid id);
        List<TransactionHistoryViewModel> GetInRangeTime(DateTime startTime, DateTime endTime);
        List<TransactionHistoryViewModel> GetByWalletId(Guid walletId);
        List<TransactionHistoryViewModel> FilterGetTransactionHistory(Guid walletId, int? filterOption);
    }
    public class TransactionHistoryService : BaseService<TransactionHistory>, ITransactionHistoryService
    {
        private readonly IConfigurationProvider _mapper;
        public TransactionHistoryService(IUnitOfWork unitOfWork, ITransactionHistoryRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public TransactionHistoryViewModel GetById(Guid id)
        {
            return Get().Where(tempTransactionHistory => tempTransactionHistory.Id.Equals(id)).ProjectTo<TransactionHistoryViewModel>(_mapper).FirstOrDefault();
        }

        public List<TransactionHistoryViewModel> GetByWalletId(Guid walletId)
        {
            return Get().Where(tempTransactionHistory => tempTransactionHistory.WalletId.Equals(walletId)).ProjectTo<TransactionHistoryViewModel>(_mapper).ToList();
        }

        public List<TransactionHistoryViewModel> GetInRangeTime(DateTime startTime, DateTime endTime)
        {
            return Get()
                .Where(tempTransactionHistory => tempTransactionHistory.ActionDate >= startTime && tempTransactionHistory.ActionDate <= endTime)
                .ProjectTo<TransactionHistoryViewModel>(_mapper)
                .ToList();
        }


        public List<TransactionHistoryViewModel> FilterGetTransactionHistory(Guid walletId, int? filterOption)
        {
            if (filterOption == null)
            {
                return GetByWalletId(walletId);
            }
            switch (filterOption)
            {
                case (int)FilterConstants.DateAscending:
                    return Get()
                        .Where(tempTransactionHistory => tempTransactionHistory.WalletId.Equals(walletId))
                        .OrderBy(tempTransactionHistory => tempTransactionHistory.ActionDate)
                        .ProjectTo<TransactionHistoryViewModel>(_mapper)
                        .ToList();

                case (int)FilterConstants.DateDescending:
                    return Get()
                        .Where(tempTransactionHistory => tempTransactionHistory.WalletId.Equals(walletId))
                        .OrderByDescending(tempTransactionHistory => tempTransactionHistory.ActionDate)
                        .ProjectTo<TransactionHistoryViewModel>(_mapper)
                        .ToList();

                case (int)FilterConstants.AmountAscending:
                    return Get()
                        .Where(tempTransactionHistory => tempTransactionHistory.WalletId.Equals(walletId))
                        .OrderBy(tempTransactionHistory => tempTransactionHistory.Amount)
                        .ProjectTo<TransactionHistoryViewModel>(_mapper)
                        .ToList();

                case (int)FilterConstants.AmountDescending:
                    return Get()
                        .Where(tempTransactionHistory => tempTransactionHistory.WalletId.Equals(walletId))
                        .OrderByDescending(tempTransactionHistory => tempTransactionHistory.Amount)
                        .ProjectTo<TransactionHistoryViewModel>(_mapper)
                        .ToList();

                default:
                    return null;
            }
        }
    }
}

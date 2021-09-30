using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Data.Enums;
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
    public interface IWalletService : IBaseService<Wallet>
    {
        WalletViewModel GetById(Guid id);
        WalletViewModel GetByMomoId(string momoId);
        WalletViewModel GetByBankId(string bankId);
        List<TransactionHistoryViewModel> GetTransactionHistory(Guid id, int pageNum, int? filterOption);
        bool UpdateAmount(Guid id, int amount, int isDeposited);
    }

    public class WalletService : BaseService<Wallet>, IWalletService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly ITransactionHistoryService _transactionHistoryService;

        private const int groupItemNum = 10;

        public WalletService(IUnitOfWork unitOfWork, IWalletRepository repository, IMapper mapper, ITransactionHistoryService transactionHistoryService) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _transactionHistoryService = transactionHistoryService;
        }

        public WalletViewModel GetById(Guid id)
        {
            return Get().Where(tempWallet => tempWallet.Id.Equals(id)).ProjectTo<WalletViewModel>(_mapper).FirstOrDefault();
        }

        public WalletViewModel GetByMomoId(string momoId)
        {
            return Get().Where(tempWallet => tempWallet.MomoId.Equals(momoId)).ProjectTo<WalletViewModel>(_mapper).FirstOrDefault();
        }

        public WalletViewModel GetByBankId(string bankId)
        {
            return Get().Where(tempWallet => tempWallet.BankId.Equals(bankId)).ProjectTo<WalletViewModel>(_mapper).FirstOrDefault();
        }

        public List<TransactionHistoryViewModel> GetTransactionHistory(Guid walletId, int pageNum, int? filterOption)
        {
            List<TransactionHistoryViewModel> transactionHistories = _transactionHistoryService.FilterGetTransactionHistory(walletId, filterOption);

            return _transactionHistoryService.GetTransactionHistoryPageItem(transactionHistories, pageNum, groupItemNum);
        }

        private bool DepositAmount(Guid id, int amount)
        {
            Wallet targetWallet = Get().Where(tempWallet => tempWallet.Id.Equals(id)).First();

            targetWallet.Balance += amount;

            try
            {
                Update(targetWallet);
                return true;
            } catch
            {
                return false;
            }
        }

        private bool DecreaseAmount(Guid id, int amount)
        {
            Wallet targetWallet = Get().Where(tempWallet => tempWallet.Id.Equals(id)).First();

            targetWallet.Balance -= amount;

            try
            {
                Update(targetWallet);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateAmount(Guid id, int amount, int status)
        {
            if (status == (int) WalletStatus.DEPOSIT)
            {
                return DepositAmount(id, amount);
            } else
            {
                return DecreaseAmount(id, amount);
            }
        }
    }
}

using AutoMapper;
using AutoMapper.Configuration;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IWalletService : IBaseService<Wallet>
    {
        Task<Wallet> CreateNew(WalletCreateRequest walletRequest);
        WalletViewModel GetById(Guid id);
        WalletViewModel GetByMomoId(string momoId);
        WalletViewModel GetByBankId(string bankId);
        List<TransactionHistoryViewModel> GetTransactionHistory(Guid id, int pageNum, int? filterOption);
        Task<bool> UpdateAmount(Guid id, int amount, int isDeposited);
    }

    public class WalletService : BaseService<Wallet>, IWalletService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly ITransactionHistoryService _transactionHistoryService;
        private readonly IOwnerService _ownerService;

        //private const int groupItemNum = 10;

        public WalletService(IUnitOfWork unitOfWork, IWalletRepository repository, IMapper mapper, 
            ITransactionHistoryService transactionHistoryService, 
            IOwnerService ownerService) : base(unitOfWork, repository)
        {
            _ownerService = ownerService;
            _transactionHistoryService = transactionHistoryService;
            _mapper = mapper.ConfigurationProvider;
        }

        public WalletViewModel GetById(Guid id)
        {
            return Get().Where(tempWallet => tempWallet.Id.Equals(id))
                .ProjectTo<WalletViewModel>(_mapper)
                .FirstOrDefault();
        }

        public WalletViewModel GetByMomoId(string momoId)
        {
            return Get().Where(tempWallet => tempWallet.MomoId.Equals(momoId))
                .ProjectTo<WalletViewModel>(_mapper)
                .FirstOrDefault();
        }

        public WalletViewModel GetByBankId(string bankId)
        {
            return Get().Where(tempWallet => tempWallet.BankId.Equals(bankId)).ProjectTo<WalletViewModel>(_mapper).FirstOrDefault();
        }

        public List<TransactionHistoryViewModel> GetTransactionHistory(Guid walletId, int pageNum, int? filterOption)
        {
            List<TransactionHistoryViewModel> transactionHistories = _transactionHistoryService.FilterGetTransactionHistory(walletId, filterOption);

            return PagingUtil<TransactionHistoryViewModel>.Paging(transactionHistories, pageNum);       
        }

        private async Task<bool> DepositAmount(Guid id, int amount)
        {
            Wallet targetWallet = Get().Where(tempWallet => tempWallet.Id.Equals(id)).First();

            targetWallet.Balance += amount;

            try
            {
                await UpdateAsync(targetWallet);
                return true;
            } catch
            {
                return false;
            }
        }

        private async Task<bool> DecreaseAmount(Guid id, int amount)
        {
            Wallet targetWallet = Get().Where(tempWallet => tempWallet.Id.Equals(id)).First();

            targetWallet.Balance -= amount;

            try
            {
                await UpdateAsync(targetWallet);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAmount(Guid id, int amount, int status)
        {
            if (status == (int)WalletStatus.DEPOSIT)
            {
                return await DepositAmount(id, amount);
            } else
            {
                return await DecreaseAmount(id, amount);
            }
        }

        public async Task<Wallet> CreateNew(WalletCreateRequest walletRequest)
        {
            var ownerView = await _ownerService.GetOwnerById(walletRequest.Id);

            if (ownerView != null && ownerView.ToString().Trim().Length > 0)
            {
                Wallet wallet = _mapper.CreateMapper().Map<Wallet>(walletRequest);
                await CreateAsync(wallet);
                return wallet;
            }
            return null;
        }
    }
}

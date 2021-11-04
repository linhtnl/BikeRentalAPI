using AutoMapper;
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
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IWalletService : IBaseService<Wallet>
    {
        Task<Wallet> CreateNew(WalletCreateRequest walletRequest);
        WalletViewModel GetById(Guid id);
        WalletViewModel GetByMomoId(string momoId);
        WalletViewModel GetByBankId(string bankId);
        Task<DynamicModelResponse<TransactionHistoryViewModel>> GetTransactionHistory(Guid walletId, int size, int pageNum, int? filterOption);
        Task<bool> UpdateAmount(Guid walletId, decimal amount, int status, Guid bookingId);
    }

    public class WalletService : BaseService<Wallet>, IWalletService
    {
        private readonly IConfigurationProvider _mapper;
        private readonly ITransactionHistoryService _transactionHistoryService;
        private readonly IOwnerRepository _ownerRepository;

        public WalletService(IUnitOfWork unitOfWork, IWalletRepository repository, IMapper mapper, 
            ITransactionHistoryService transactionHistoryService, 
            IOwnerRepository ownerRepository) : base(unitOfWork, repository)
        {
            _ownerRepository = ownerRepository;
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

        public async Task<DynamicModelResponse<TransactionHistoryViewModel>> GetTransactionHistory(Guid walletId, int size, int pageNum, int? filterOption)
        {
            List<TransactionHistoryViewModel> transactionHistories = _transactionHistoryService.FilterGetTransactionHistory(walletId, filterOption);

            var result = transactionHistories.AsQueryable().PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
            if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var rs = new DynamicModelResponse<TransactionHistoryViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = pageNum,
                    Size = size,
                    Total = result.Item1
                },
                Data = result.Item2.ToList()
            };
            return await Task.Run(()=> rs);
        }

        private async Task<bool> DepositAmount(Guid walletId, decimal amount, Guid bookingId)
        {
            Wallet targetWallet = await GetAsync(walletId);

            targetWallet.Balance += amount;

            try
            {
                await UpdateAsync(targetWallet);

                await _transactionHistoryService.CreateNew(walletId, bookingId, DateTime.Now, false, amount);

                return true;
            } catch
            {
                return false;
            }
        }

        private async Task<bool> DecreaseAmount(Guid walletId, decimal amount, Guid bookingId)
        {
            Wallet targetWallet = await GetAsync(walletId);

            targetWallet.Balance -= amount;

            try
            {
                await UpdateAsync(targetWallet);

                await _transactionHistoryService.CreateNew(walletId, bookingId, DateTime.Now, true, amount);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAmount(Guid walletId, decimal amount, int status, Guid bookingId)
        {
            if (status == (int)WalletStatus.DEPOSIT)
            {
                return await DepositAmount(walletId, amount, bookingId);
            } else
            {
                return await DecreaseAmount(walletId, amount, bookingId);
            }
        }

        public async Task<Wallet> CreateNew(WalletCreateRequest walletRequest)
        {
            var ownerView = await _ownerRepository.GetAsync(walletRequest.Id);

            if (ownerView != null)
            {
                Wallet wallet = _mapper.CreateMapper().Map<Wallet>(walletRequest);
                await CreateAsync(wallet);
                return wallet;
            }
            throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot create wallet for this owner.");
        }
    }
}

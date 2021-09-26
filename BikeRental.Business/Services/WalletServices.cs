using AutoMapper;
using AutoMapper.Configuration;
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
    public interface IWalletService : IBaseService<Wallet>
    {
        WalletViewModel GetById(Guid id);
        WalletViewModel GetByMomoId(string momoId);
        WalletViewModel GetByBankId(string bankId);
        WalletViewModel GetTransactionHistory(Guid id);
    }

    public class WalletService : BaseService<Wallet>, IWalletService
    {
        private readonly IConfigurationProvider _mapper;

        public WalletService(IUnitOfWork unitOfWork, IWalletRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
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

        public WalletViewModel GetTransactionHistory(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

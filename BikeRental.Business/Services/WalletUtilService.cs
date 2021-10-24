using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IWalletUtilService : IBaseService<Wallet>
    {
        public Task<decimal?> GetWalletBalance(Guid id);
    }
    public class WalletUtilService : BaseService<Wallet>, IWalletUtilService
    {
        public WalletUtilService(IUnitOfWork unitOfWork, IWalletRepository repository) : base(unitOfWork, repository)
        {

        }

        public async Task<decimal?> GetWalletBalance(Guid id)
        {
            var wallet = await Get(w => w.Id.Equals(id)).FirstOrDefaultAsync();
            return wallet.Balance;
        }
    }
}

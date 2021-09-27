using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface ITransactionHistoryRepository : IBaseRepository<TransactionHistory>
    {
    }
    public class TransactionHistoryRepository : BaseRepository<TransactionHistory>, ITransactionHistoryRepository
    {
        public TransactionHistoryRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

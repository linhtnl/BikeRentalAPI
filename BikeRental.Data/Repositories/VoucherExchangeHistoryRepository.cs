using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface IVoucherExchangeHistoryRepository : IBaseRepository<VoucherExchangeHistory>
    {

    }
    public class VoucherExchangeHistoryRepository : BaseRepository<VoucherExchangeHistory>, IVoucherExchangeHistoryRepository
    {
        public VoucherExchangeHistoryRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

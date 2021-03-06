using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface IVoucherItemRepository : IBaseRepository<VoucherItem>
    {
    }
    public class VoucherItemRepository : BaseRepository<VoucherItem>, IVoucherItemRepository
    {
        public VoucherItemRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

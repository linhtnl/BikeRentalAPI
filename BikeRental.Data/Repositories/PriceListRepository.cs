using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface IPriceListRepository : IBaseRepository<PriceList>
    {

    }
    public class PriceListRepository : BaseRepository<PriceList>, IPriceListRepository
    {
        public PriceListRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}

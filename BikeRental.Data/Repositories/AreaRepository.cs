
using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface IAreaRepository : IBaseRepository<Area>
    {
    }
    public class AreaRepository : BaseRepository<Area>, IAreaRepository
    {
        public AreaRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

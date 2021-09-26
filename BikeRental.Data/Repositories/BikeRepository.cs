using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface IBikeRepository : IBaseRepository<Bike> { 
    }
    public class BikeRepository : BaseRepository<Bike> , IBikeRepository
    {
        public BikeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

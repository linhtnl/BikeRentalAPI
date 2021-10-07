using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
    }
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

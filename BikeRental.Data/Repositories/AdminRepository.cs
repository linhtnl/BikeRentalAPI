
using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface IAdminRepository : IBaseRepository<Admin>
    {
    }
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    { 
        public AdminRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
 }
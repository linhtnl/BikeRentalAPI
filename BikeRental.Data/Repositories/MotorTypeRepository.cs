using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface IMotorTypeRepository : IBaseRepository<MotorType>
    {

    }
    public class MotorTypeRepository : BaseRepository<MotorType>, IMotorTypeRepository
    {
        public MotorTypeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

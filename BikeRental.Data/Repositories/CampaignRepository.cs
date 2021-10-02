using BikeRental.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repositories
{
    public interface ICampaignRepository : IBaseRepository<Campaign>
    {
    }
    public class CampaignRepository : BaseRepository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

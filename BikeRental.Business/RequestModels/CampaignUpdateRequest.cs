using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class CampaignUpdateRequest
    {
        public string Description { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? StartingDate { get; set; }
    }
}

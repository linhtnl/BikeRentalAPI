using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Models.Request
{
    public class CampaignRequest
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid? AreaId { get; set; }
        public bool? IsHappening { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartingDate { get; set; }
    }
}

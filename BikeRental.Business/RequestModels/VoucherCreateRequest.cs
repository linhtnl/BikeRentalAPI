using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class VoucherCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartingDate { get; set; }
        public int? VoucherItemsRemain { get; set; }
        public Guid? CampaignId { get; set; }
    }
}

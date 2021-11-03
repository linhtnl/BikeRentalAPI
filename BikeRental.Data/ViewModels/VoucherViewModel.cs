using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class VoucherViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? Status { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartingDate { get; set; }
        public int? VoucherItemsRemain { get; set; }
        public Guid? CampaignId { get; set; }
        public int? NumberOfUses { get; set; }
        public int? PointExchange { get; set; }
    }
}

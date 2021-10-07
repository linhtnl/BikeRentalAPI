using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            VoucherItems = new HashSet<VoucherItem>();
        }

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

        public virtual Campaign Campaign { get; set; }
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class VoucherItem
    {
        public VoucherItem()
        {
            Bookings = new HashSet<Booking>();
        }

        public string Id { get; set; }
        public DateTime? TimeUsing { get; set; }
        public int? TimeUsingRemain { get; set; }
        public string CustomerId { get; set; }
        public int? PointExchange { get; set; }
        public string VoucherId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Voucher Voucher { get; set; }
        public virtual VoucherExchangeHistory VoucherExchangeHistory { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}

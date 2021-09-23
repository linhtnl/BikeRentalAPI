using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class VoucherExchangeHistory
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string VoucherCode { get; set; }
        public DateTime? ActionDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual VoucherItem VoucherCodeNavigation { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class VoucherExchangeHistory
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? VoucherCode { get; set; }
        public DateTime? ActionDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual VoucherItem VoucherCodeNavigation { get; set; }
    }
}

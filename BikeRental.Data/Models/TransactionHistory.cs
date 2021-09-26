using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class TransactionHistory
    {
        public Guid Id { get; set; }
        public DateTime? ActionDate { get; set; }
        public bool? Action { get; set; }
        public decimal? Amount { get; set; }
        public Guid? WalletId { get; set; }
        public Guid? BookingId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}

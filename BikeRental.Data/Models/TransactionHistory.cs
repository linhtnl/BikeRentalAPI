using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class TransactionHistory
    {
        public string Id { get; set; }
        public DateTime? ActionDate { get; set; }
        public bool? Action { get; set; }
        public decimal? Amount { get; set; }
        public string WalletId { get; set; }
        public string BookingId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}

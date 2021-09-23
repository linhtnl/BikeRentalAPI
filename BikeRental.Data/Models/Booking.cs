using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Booking
    {
        public Booking()
        {
            TransactionHistories = new HashSet<TransactionHistory>();
        }

        public string Id { get; set; }
        public DateTime? DayRent { get; set; }
        public DateTime? DayReturnActual { get; set; }
        public DateTime? DayReturnExpected { get; set; }
        public decimal? Price { get; set; }
        public string VoucherCode { get; set; }
        public string CustomerId { get; set; }
        public string BikeId { get; set; }
        public string OwnerId { get; set; }
        public string PaymentId { get; set; }

        public virtual Bike Bike { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Owner Owner { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual VoucherItem VoucherCodeNavigation { get; set; }
        public virtual Feedback Feedback { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistories { get; set; }
    }
}

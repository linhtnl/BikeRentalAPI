using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Bookings = new HashSet<Booking>();
            VoucherExchangeHistories = new HashSet<VoucherExchangeHistory>();
            VoucherItems = new HashSet<VoucherItem>();
        }

        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }
        public int? RewardPoints { get; set; }
        public int? Status { get; set; }
        public int? BanCount { get; set; }
        public Guid? AdminId { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<VoucherExchangeHistory> VoucherExchangeHistories { get; set; }
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}

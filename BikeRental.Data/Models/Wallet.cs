using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            TransactionHistories = new HashSet<TransactionHistory>();
        }

        public string Id { get; set; }
        public decimal? Balance { get; set; }
        public string MomoId { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }

        public virtual Owner IdNavigation { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistories { get; set; }
    }
}

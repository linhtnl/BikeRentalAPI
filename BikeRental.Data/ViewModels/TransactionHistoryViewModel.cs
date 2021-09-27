using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class TransactionHistoryViewModel
    {
        public Guid Id { get; set; }
        public DateTime? ActionDate { get; set; }
        public bool? Action { get; set; }
        public decimal? Amount { get; set; }
        public Guid? WalletId { get; set; }
        public Guid? BookingId { get; set; }
    }
}

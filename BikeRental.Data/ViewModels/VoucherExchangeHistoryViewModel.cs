using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class VoucherExchangeHistoryViewModel
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? VoucherCode { get; set; }
        public DateTime? ActionDate { get; set; }
    }
}

using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class VoucherItemCreateSuccessViewModel
    {
        public Guid Id { get; set; }
        public int? TimeUsingRemain { get; set; }
        public Guid? CustomerId { get; set; }
        public int? PointExchange { get; set; }
        public Guid? VoucherId { get; set; }
        public VoucherExchangeHistoryViewModel voucherExchange { get; set; }
    }
}

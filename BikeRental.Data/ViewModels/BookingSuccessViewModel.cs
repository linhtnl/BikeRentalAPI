using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BookingSuccessViewModel
    {
        public Guid Id { get; set; }

        public Guid? VoucherCode { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? BikeId { get; set; }

        public DateTime? DayRent { get; set; }

        public DateTime? DayReturnExpected { get; set; }

        public decimal? Price { get; set; }

        public Guid? PaymentId { get; set; }

        public int? Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class BookingCreateRequest
    {
        public Guid? VoucherCode { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? BikeId { get; set; }

        public DateTime? DayRent { get; set; }

        public DateTime? DayReturnActual { get; set; }

        public DateTime? DayReturnExpected { get; set; }

        public decimal? Price { get; set; }

        public Guid? PaymentId { get; set; }
    }
}

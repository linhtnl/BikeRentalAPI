using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BookingDetailViewModel
    {
        public Guid Id { get; set; }

        public Guid? VoucherCode { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? BikeId { get; set; }

        public DateTime? DayRent { get; set; }

        public DateTime? DayReturnActual { get; set; }

        public DateTime? DayReturnExpected { get; set; }
        public string CustomerName { get; set; }
        public string OwnerName { get; set; }
        public string PhoneNum { get; set; }

        public decimal? Price { get; set; }

        public string Address { get; set; }

        public Guid? PaymentId { get; set; }

        public int? Status { get; set; }
        public BikeByIdViewModel Bike { get; set; }
    }
}

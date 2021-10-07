using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BookingViewModel
    {
        [BindNever]
        public Guid Id { get; set; }
        [BindNever]
        public DateTime? DayRent { get; set; }
        [BindNever]
        public DateTime? DayReturnActual { get; set; }
        [BindNever]
        public DateTime? DayReturnExpected { get; set; }
        [BindNever]
        public decimal? Price { get; set; }
        [BindNever]
        public Guid? VoucherCode { get; set; }
        [BindNever]
        public Guid? CustomerId { get; set; }
        [BindNever]
        public Guid? BikeId { get; set; }
        [BindNever]
        public Guid? OwnerId { get; set; }
        [BindNever]
        public Guid? PaymentId { get; set; }
        public int? Status { get; set; }
    }
}

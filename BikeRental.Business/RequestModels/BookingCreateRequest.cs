using System;

namespace BikeRental.Business.RequestModels
{
    public class BookingCreateRequest
    {
        public Guid? VoucherCode { get; set; }

        public string CategoryName { get; set; }

        public Guid CustomerId { get; set; }

        public Guid AreaId { get; set; }

        public DateTime DayRent { get; set; }

        public DateTime? DayReturnExpected { get; set; }

        public Guid? PaymentId { get; set; }
    }
}

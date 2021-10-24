using System;

namespace BikeRental.Business.RequestModels
{
    public class BookingCreateRequest
    {
        public Guid OwnerId { get; set; }

        public Guid BikeId { get; set; }

        public Guid CategoryId { get; set; }

        public Guid TypeId { get; set; }

        public Guid PaymentId { get; set; }

        public Guid? VoucherCode { get; set; }

        public decimal Price { get; set; }
        
        public DateTime DayRent { get; set; }

        public DateTime DayReturnExpected { get; set; }
    }
}

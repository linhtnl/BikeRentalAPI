using System;

namespace BikeRental.Business.RequestModels
{
    public class WalletRequest
    {
        public Guid WalletId { get; set; }
        public int Amount { get; set; }
        public Guid BookingId { get; set; }
    }
}

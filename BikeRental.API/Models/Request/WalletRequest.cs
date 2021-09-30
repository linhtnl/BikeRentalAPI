using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Models.Request
{
    public class WalletRequest
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
    }
}

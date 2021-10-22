using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class CustomerRequestModel
    {
        public string LicensePlate { get; set; }
        public DateTime DateRent { get; set; }
        public DateTime DateReturn { get; set; }
    }
}

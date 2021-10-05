using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class BikeUpdateRequest
    {
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public string ModelYear { get; set; }
    }
}

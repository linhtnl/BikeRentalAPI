using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class BikeUpdateRequest
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public string ModelYear { get; set; }
        public string ImgPath { get; set; }
        public int? Status { get; set; }
    }
}

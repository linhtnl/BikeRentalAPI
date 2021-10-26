using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class CustomerRequestModel
    {
        public string CustomerName { get; set; }
        public string LicensePlate { get; set; }
        public string CateName { get;set; }
        public DateTime DateRent { get; set; }
        public DateTime DateReturn { get; set; }

        public string ImgPath { get; set; }
        public string Address { get; set; }
        public double Price { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class DistanceViewModel
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? Distance { get; set; }
    }
}

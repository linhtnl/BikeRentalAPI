using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BikeFindingViewModel
    {
        public Guid? Id { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? CategoryId { get; set; }
        public int TotalBike { get; set; }

        public string CateName { get; set; }

        public string Color { get; set; }

        public string ModelYear { get; set; }

        public string LicensePlate { get; set; }

        public int? Status { get; set; }

        public string ImgPath { get; set; }

        public double? Rating { get; set; }
        public int? NumberOfRating { get; set; }
    }
}

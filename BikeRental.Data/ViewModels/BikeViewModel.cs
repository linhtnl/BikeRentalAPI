using BikeRental.Business.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BikeViewModel
    {
        public Guid? Id { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? CategoryId { get; set; }
        [String]
        public string BrandName { get; set; }
        [String]
        public string CategoryName { get; set; }
        [String]
        public string Color { get; set; }
        [String]
        public string ModelYear { get; set; }
        [String]
        public string LicensePlate { get; set; }
        public int? Status { get; set; }

        //lưu hình
    }
}

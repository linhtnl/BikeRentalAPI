using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BikeViewModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid CategoryId { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string OwnerPhone { get; set; }
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public int? Status { get; set; }
        public string ModelYear { get; set; }
        public double? Rating { get; set; }
        public int? NumberOfRating { get; set; }
        //lưu hình
    }
}

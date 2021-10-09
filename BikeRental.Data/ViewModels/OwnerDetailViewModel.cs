using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class OwnerDetailViewModel
    {
        public Guid Id { get; set; }
        public Guid? AreaId { get; set; }
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }      
        public double? Rating { get; set; }
        public int? NumberOfRatings { get; set; }
        public int? BanTimes { get; set; }
        public List<BikeViewModel>? ListBike { get; set; }
    }
}

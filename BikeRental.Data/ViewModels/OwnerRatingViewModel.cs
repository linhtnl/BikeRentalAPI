using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Models
{
    public class OwnerRatingViewModel
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public int NumberOfBikes { get; set; }
        public string IdentityImg { get; set; }
        public double? Rating { get; set; }
        public int? NumberOfRatings { get; set; }
        public Guid? AreaId { get; set; }
    }
}
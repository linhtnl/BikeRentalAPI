using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }
        public int? RewardPoints { get; set; }
        public int Status { get; set; }
        public int? BanCount { get; set; }
    }
}

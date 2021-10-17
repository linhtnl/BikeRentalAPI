using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class OwnerViewModel
    {
        public Guid Id { get; set; }
        public Guid? AdminId { get; set; }
        public Guid? AreaId { get; set; }
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; }
        public int? BanTimes { get; set; }
        public string Mail { get; set; }
        public string ImgPath { get; set; }
    }
}

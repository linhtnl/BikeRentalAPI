using BikeRental.Business.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Models
{
    public class OwnerRatingViewModel
    {
        [BindNever]
        public Guid? Id { get; set; }
        [BindNever]
        public Guid? AreaId { get; set; }
        [BindNever]
        public string PhoneNumber { get; set; }
        [String]
        public string Fullname { get; set; }
        [BindNever]
        public string Address { get; set; }
        [BindNever]
        public int? NumberOfBikes { get; set; }
        [BindNever]
        public double? Rating { get; set; }
        [BindNever]
        public int? NumberOfRatings { get; set; }
        [BindNever]
        public string ImgPath { get; set; }
       
    }
}
using BikeRental.Business.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BikeViewModel
    {
        [BindNever]
        public Guid? Id { get; set; }
        [BindNever]
        public Guid? OwnerId { get; set; }
        [BindNever]
        public Guid? CategoryId { get; set; }

        [String]
        public string BrandName { get; set; }
        [String]
        public string CategoryName { get; set; }
        [String]
        public string Color { get; set; }
        [String]
        public string ModelYear { get; set; }
        [BindNever]
        public string LicensePlate { get; set; }
        [BindNever]
        public int? Status { get; set; }

        //lưu hình
    }
}

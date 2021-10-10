using BikeRental.Business.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class CategoryViewModel
    {
        [BindNever]
        public Guid? Id { get; set; }
        [String]
        public string Name { get; set; }
        public int? Type { get; set; }
        [BindNever]
        public Guid? BrandId { get; set; }
        [String]
        public string BrandName { get; set; }
        public int? Status { get; set; }
    }
}

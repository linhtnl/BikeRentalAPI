using BikeRental.Business.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BrandViewModel
    {
        [BindNever]
        public Guid? id { get; set; }
        [String]
        public string Name { get; set; }
        public int Status { get; set; }
    }
}

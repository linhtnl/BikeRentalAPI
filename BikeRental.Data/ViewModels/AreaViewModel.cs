using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeRental.Business.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BikeRental.Data.ViewModels
{
    public class AreaViewModel
    {
        [BindNever]
        public Guid? Id { get; set; }
        [String]
        public string PostalCode { get; set; }
        [String]
        public string Name { get; set; }
        [BindNever]
        public int Status { get; set; }
    }
}

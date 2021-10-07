﻿using BikeRental.Business.Attributes;
using BikeRental.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class PriceListViewModel
    {
        [BindNever]
        public Guid? CategoryId { get; set; }
        [BindNever]
        public Guid? AreaId { get; set; }
        [String]
        public string CateName { get; set; }
        [BindNever]
        public string BrandName { get; set; }
        [BindNever]
        public decimal? Price { get; set; }
        
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class OwnerByAreaViewModel
    {
        public Guid Id { get; set; }
        public double? Rating { get; set; }
        public List<BikeViewModel>? ListBike { get; set; }
    }
}

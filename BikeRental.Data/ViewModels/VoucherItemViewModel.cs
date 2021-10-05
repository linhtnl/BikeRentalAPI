﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{   
    public class VoucherItemViewModel
    {
        public Guid Id { get; set; }
        public DateTime? TimeUsing { get; set; }
        public int? TimeUsingRemain { get; set; }
        public Guid? CustomerId { get; set; }
        public int? PointExchange { get; set; }
        public Guid? VoucherId { get; set; }
    }
}
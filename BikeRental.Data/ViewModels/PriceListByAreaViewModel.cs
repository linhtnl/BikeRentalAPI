using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class PriceListByAreaViewModel
    {

        public Guid MotorTypeId { get; set; }
        public Guid? AreaId { get; set; }
        public string TypeName { get; set; }
        public decimal? Price { get; set; }
    }
}

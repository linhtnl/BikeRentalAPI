using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class CategoryCreateModel
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public Guid BrandId { get; set; }
    }
}

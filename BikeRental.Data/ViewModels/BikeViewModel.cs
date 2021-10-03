using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BikeViewModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid CategoryId { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }
        public string ModelYear { get; set; }
        public int? Status { get; set; }

        //lưu hình
    }
}

using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Interface
{
    public interface IArea :IRepository<Area>
    {
        public Area GetByName(string name);

        public Area GetByPostalCode(int postalCode);
    }
}

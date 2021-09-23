using BikeRental.Data.Interface;
using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Service
{
    public class AreaService
    {
        private readonly IArea _area;

        public AreaService(IArea area)
        {
            _area = area;
        }

        public IEnumerable<Area> getAll()
        {
            try
            {
                return _area.GetAll().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Area getAreaById(string id)
        {
            try
            {
                return _area.GetByID(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Area getAreaByName(string name)
        {
            try
            {
                return _area.GetByName(name);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Area getAreaByPostalCode(int postalCode)
        {
            try
            {
                return _area.GetByPostalCode(postalCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

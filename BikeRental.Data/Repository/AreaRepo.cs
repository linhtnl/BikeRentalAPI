using BikeRental.Data.Interface;
using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repository
{
    public class AreaRepo : IArea
    {
        ChoThueXeMayContext _dbContext;
        public AreaRepo(ChoThueXeMayContext context)
        {
            _dbContext = context;
        }

        public Task<Area> Create(Area _object)
        {
            throw new NotImplementedException();
        }

        public void Delete(Area _object)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Area> GetAll()
        {
            try
            {
                return _dbContext.Areas.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Area GetByID(string id)
        {
            try
            {
                return _dbContext.Areas.Where(area => area.Id.Equals(id)).First();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Area _object)
        {
            throw new NotImplementedException();
        }

        public Area GetByPostalCode(int postalCode)
        {
            try
            {
                return _dbContext.Areas.Where(area => area.PostalCode == postalCode).First();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Area GetByName(string name)
        {
            try
            {
                return _dbContext.Areas.Where(area => area.Name.Equals(name)).First();
            }
            catch
            {
                throw;
            }
        }
    }
}

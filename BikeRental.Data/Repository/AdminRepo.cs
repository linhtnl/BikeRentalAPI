using BikeRental.Data.Interface;
using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.Repository
{
    public class AdminRepo : IRepository<Admin>
    {
        ChoThueXeMayContext _dbContext;

        public AdminRepo(ChoThueXeMayContext context)
        {
            _dbContext = context;
        }

        public async Task<Admin> Create(Admin _object)
        {
            var obj = await _dbContext.Admins.AddAsync(_object);
            _dbContext.SaveChanges();
            return obj.Entity;
        }

        public void Delete(Admin _object)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Admin> GetAll()
        {
            try
            {
                return _dbContext.Admins.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Admin GetByID(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Admin _object)
        {
            _dbContext.Admins.Update(_object);
            _dbContext.SaveChanges();
        }
    }
}

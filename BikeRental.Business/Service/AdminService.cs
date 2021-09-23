using BikeRental.Data.Interface;
using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Service
{
    public class AdminService
    {
        private readonly IRepository<Admin> _admin;

        public AdminService(IRepository<Admin> admin)
        {
            _admin = admin;
        }
        //GET All Admin Details   
        public IEnumerable<Admin> GetAllAdmins()
        {
            try
            {
                return _admin.GetAll().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Add Admin
        public async Task<Admin> AddAdmin(Admin admin)
        {
            return await _admin.Create(admin);
        }

        // Update Admin
        public bool Update(Admin admin)
        {
            try
            {
                _admin.Update(admin);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}

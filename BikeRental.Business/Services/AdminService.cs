using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeRental.Data.ViewModels;
using AutoMapper;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.Repositories;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.RequestModels;
using FirebaseAdmin.Auth;
using BikeRental.Data.Responses;
using BikeRental.Business.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BikeRental.Business.Services
{
    public interface IAdminService : IBaseService<Admin>
    {
        List<CustomerViewModel> GetAllCustomer();
        CustomerViewModel GetCustomerById(Guid id);
        CustomerViewModel GetCustomerByPhone(string phone);
        Task<string> Login(AdminLoginRequest request, IConfiguration configuration);
    }
    public class AdminService : BaseService<Admin>, IAdminService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly ICustomerRepository _customerRepository;
        public AdminService(IUnitOfWork unitOfWork, IAdminRepository adminRepository,
            ICustomerRepository customerRepository, IMapper mapper) : base(unitOfWork, adminRepository)
        {
            _customerRepository = customerRepository;
            _mapper = mapper.ConfigurationProvider;
        }

        public List<CustomerViewModel> GetAllCustomer()
        {
            return _customerRepository.Get().ProjectTo<CustomerViewModel>(_mapper).ToList();
        }

        public CustomerViewModel GetCustomerById(Guid id)
        {
            return _customerRepository.Get(c => c.Id.Equals(id)).ProjectTo<CustomerViewModel>(_mapper).FirstOrDefault();
        }

        public CustomerViewModel GetCustomerByPhone(string phone)
        {
            return _customerRepository.Get(c => c.PhoneNumber.Equals(phone)).ProjectTo<CustomerViewModel>(_mapper).FirstOrDefault();
        }

        public async Task<string> Login(AdminLoginRequest request, IConfiguration configuration)
        {
            string username = request.Username;
            string password = request.Password;

            var adminResult = await Get(adminTemp => (adminTemp.UserName.Equals(username) && adminTemp.Password.Equals(password))).ProjectTo<AdminViewModel>(_mapper).FirstOrDefaultAsync();
            if (adminResult == null) throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Wrong username or password");

            string token = new TokenService(configuration).GenerateAdminJWTWebToken(adminResult);

            return await Task.Run(() => token);
        }
    }
}

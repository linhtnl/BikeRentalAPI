using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Data.Enums;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Task<Customer> CreateNew(CustomerCreateRequest request);
        Task<CustomerViewModel> GetCustomerById(Guid id);
        CustomerViewModel GetCustomerByPhone(string phone);
        Task<UserLoginResponseViewModel> Login(string phoneNumber, IConfiguration configuration);
        Task<UserLoginResponseViewModel> Register(CustomerCreateRequest request, IConfiguration configuration);
        Task<CustomerViewModel> DeleteCustomer(Guid id, string token);
        Task<CustomerViewModel> UpdateCustomer(CustomerUpdateRequest request);
    }
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, 
            ICustomerRepository repository) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;
        }

        public async Task<Customer> CreateNew(CustomerCreateRequest request)
        {
            Customer targetCustomer = _mapper.CreateMapper().Map<Customer>(request);
            await CreateAsync(targetCustomer);

            return targetCustomer;
        }

        public async Task<CustomerViewModel> DeleteCustomer(Guid id, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            int role = tokenModel.Role;
            if (role != (int)RoleConstants.Admin)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");

            var customer = await GetAsync(id);
            if (customer == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Customer not found.");

            customer.Status = (int)UserStatus.Deactive;
            customer.BanCount += 1;
            await UpdateAsync(customer);

            var result = _mapper.CreateMapper().Map<CustomerViewModel>(customer);

            return await Task.Run(() => result);
        }

        public async Task<CustomerViewModel> GetCustomerById(Guid id)
        {
            return await Get(c => c.Id.Equals(id)).ProjectTo<CustomerViewModel>(_mapper).FirstOrDefaultAsync();
        }

        public CustomerViewModel GetCustomerByPhone(string phone)
        {
            return Get(c => c.PhoneNumber.Equals(phone)).ProjectTo<CustomerViewModel>(_mapper).FirstOrDefault();
        }

        public async Task<UserLoginResponseViewModel> Login(string phoneNumber, IConfiguration configuration)
        {
            CustomerViewModel customer = GetCustomerByPhone(phoneNumber);

            if (customer == null)
                throw new ErrorResponse((int)HttpStatusCode.NotFound, "Phone number not existed in database yet.");

            if (customer.Status == (int)UserStatus.Deactive)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This user has been banned.");

            string token = TokenService.GenerateCustomerJWTWebToken(customer, configuration);

            var response = new UserLoginResponseViewModel(token, customer.Fullname);

            return await Task.Run(() => response);
        }

        public async Task<UserLoginResponseViewModel> Register(CustomerCreateRequest request, IConfiguration configuration)
        {
            CustomerViewModel targetCustomer = GetCustomerByPhone(request.PhoneNumber);

            if (targetCustomer != null)
            {
                throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Phone number existed.");
            }
            Customer newCustomer = await CreateNew(request);

            targetCustomer = _mapper.CreateMapper().Map<CustomerViewModel>(newCustomer);

            string token = TokenService.GenerateCustomerJWTWebToken(targetCustomer, configuration);

            var response = new UserLoginResponseViewModel(token, targetCustomer.Fullname);

            return await Task.Run(() => response);
        }

        public async Task<CustomerViewModel> UpdateCustomer(CustomerUpdateRequest request)
        {
            try
            {
                var customer = await GetAsync(request.Id);
                if (customer == null) 
                    throw new ErrorResponse((int)HttpStatusCode.NotFound, "Customer not found.");

                await UpdateAsync(customer);
                CustomerViewModel updatedCustomer = _mapper.CreateMapper().Map<CustomerViewModel>(customer);
                return await Task.Run(() => updatedCustomer);
            }
            catch
            {
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Something went wrong.");
            }
        }
    }
}

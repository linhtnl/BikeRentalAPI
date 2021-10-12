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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Task<Customer> CreateNew(CustomerCreateRequest request);
        CustomerViewModel GetCustomerById(Guid id);
        CustomerViewModel GetCustomerByPhone(string phone);
        Task<BikeViewModel> ViewBike(Guid id);
        Task<string> Login(string phoneNumber, IConfiguration configuration);
        Task<string> Register(CustomerCreateRequest request, IConfiguration configuration);
        Task<bool> DeleteCustomer(Guid id);
        Task<CustomerViewModel> UpdateCustomer(CustomerUpdateRequest request);
    }
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IBikeService _bikeService;
        public CustomerService(IUnitOfWork unitOfWork, IBikeService bikeService, ICustomerRepository repository, 
            IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _bikeService = bikeService;
        }

        public async Task<Customer> CreateNew(CustomerCreateRequest request)
        {
            Customer targetCustomer = _mapper.CreateMapper().Map<Customer>(request);
            await CreateAsync(targetCustomer);

            return targetCustomer;
        }

        public async Task<bool> DeleteCustomer(Guid id)
        {
            try
            {
                var customer = await GetAsync(id);
                if (customer == null) throw new ErrorResponse((int)ResponseStatusConstants.NOT_FOUND, "Customer not found.");

                customer.Status = (int)UserStatus.Deactive;
                customer.BanCount += 1;
                await UpdateAsync(customer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public CustomerViewModel GetCustomerById(Guid id)
        {
            return Get(c => c.Id.Equals(id)).ProjectTo<CustomerViewModel>(_mapper).FirstOrDefault();
        }

        public CustomerViewModel GetCustomerByPhone(string phone)
        {
            return Get(c => c.PhoneNumber.Equals(phone)).ProjectTo<CustomerViewModel>(_mapper).FirstOrDefault();
        }

        public async Task<string> Login(string phoneNumber, IConfiguration configuration)
        {
            CustomerViewModel customer = GetCustomerByPhone(phoneNumber);

            if (customer == null)
            {
                throw new ErrorResponse((int)ResponseStatusConstants.CREATED, "Phone number not existed in database yet.");
            }
            string token = TokenService.GenerateCustomerJWTWebToken(customer, configuration);

            return await Task.Run(() => token);
        }

        public async Task<string> Register(CustomerCreateRequest request, IConfiguration configuration)
        {
            CustomerViewModel targetCustomer = GetCustomerByPhone(request.PhoneNumber);

            if (targetCustomer != null)
            {
                throw new ErrorResponse((int)ResponseStatusConstants.BAD_REQUEST, "Phone number existed.");
            }
            Customer newCustomer = await CreateNew(request);

            targetCustomer = _mapper.CreateMapper().Map<CustomerViewModel>(newCustomer);

            string token = TokenService.GenerateCustomerJWTWebToken(targetCustomer, configuration);

            return await Task.Run(() => token);
        }

        public async Task<CustomerViewModel> UpdateCustomer(CustomerUpdateRequest request)
        {
            try
            {
                var customer = await GetAsync(request.Id);
                if (customer == null) throw new ErrorResponse((int)ResponseStatusConstants.NOT_FOUND, "Customer not found.");

                await UpdateAsync(customer);
                CustomerViewModel updatedCustomer = _mapper.CreateMapper().Map<CustomerViewModel>(customer);
                return await Task.Run(() => updatedCustomer);
            }
            catch
            {
                throw new ErrorResponse((int)ResponseStatusConstants.FORBIDDEN, "Something went wrong.");
            }
        }

        public async Task<BikeViewModel> ViewBike(Guid id)
        {
            return await _bikeService.GetBikeById(id);
        }
    }
}

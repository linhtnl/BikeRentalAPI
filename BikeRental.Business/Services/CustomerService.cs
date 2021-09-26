using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface ICustomerService : IBaseService<Customer>
    {
        
        CustomerViewModel GetCustomerById(Guid id);

        CustomerViewModel GetCustomerByPhone(string phone);

    }
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly IConfigurationProvider _mapper;
        public CustomerService(IUnitOfWork unitOfWork , ICustomerRepository repository,
            IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public CustomerViewModel GetCustomerById(Guid id)
        {
            return Get(c => c.Id.Equals(id)).ProjectTo<CustomerViewModel>(_mapper).FirstOrDefault();
        }

        public CustomerViewModel GetCustomerByPhone(string phone)
        {
            return Get(c => c.PhoneNumber.Equals(phone)).ProjectTo<CustomerViewModel>(_mapper).FirstOrDefault();
        }
    }
}

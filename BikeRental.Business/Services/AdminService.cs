
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

namespace BikeRental.Business.Services
{
    public interface IAdminService : IBaseService<Admin>
    {
        List<CustomerViewModel> GetAllCustomer();

        CustomerViewModel GetCustomerById(Guid id);

        CustomerViewModel GetCustomerByPhone(string phone);

       /* List<OwnerViewModel> GetAllOwner();*/
    }
    public class AdminService : BaseService<Admin>, IAdminService
    {
        private readonly IConfigurationProvider _mapper;
        /*private readonly IAdminRepository _repository;*/
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


        /*public List<OwnerViewModel> GetAllOwner()
        {
            
        }*/
    }
}

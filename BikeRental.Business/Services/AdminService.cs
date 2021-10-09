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
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.GoogleId); // get user by request's guid
            OwnerViewModel result = null/*await GetByMail(userRecord.Email)*/; // this should un-commented when fix database

            if (result != null) // if email existed in local database
            {
                FirebaseToken token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.AccessToken); // re-check access token with firebase
                object email;
                token.Claims.TryGetValue("email", out email); // get email from the above re-check step, then check the email whether it's matched the request email
                if (userRecord.Email.Equals(email))
                {
                    string verifyRequestToken = new TokenService(configuration).GenerateOwnerJWTWebToken(result);

                    return await Task.Run(() => verifyRequestToken); // return if everything is done
                }
                throw new ErrorResponse((int)ResponseStatusConstants.FORBIDDEN, "Email from request and the one from access token is not matched."); // return if this email's not existed yet in database - FE foward to sign up page
            }
            var claim = new Dictionary<string, object> { { "email", userRecord.Email } };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(request.GoogleId, claim);

            throw new ErrorResponse((int)ResponseStatusConstants.CREATED, "Email's not existed in database yet.");
        }
    }
}

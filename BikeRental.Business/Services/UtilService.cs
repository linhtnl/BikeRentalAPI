using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IUtilService : IBaseService<Owner>
    {
        Task<Owner> GetOwnerByOwnerId(Guid ownerId);
    }

    public class UtilService : BaseService<Owner>, IUtilService
    {
        private readonly IConfigurationProvider _mapper;

        public UtilService(IUnitOfWork unitOfWork, IOwnerRepository ownerRepository, IMapper mapper) : base(unitOfWork, ownerRepository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Owner> GetOwnerByOwnerId(Guid ownerId)
        {
            var owner = await GetAsync(ownerId);

            return await Task.Run(() => owner);
        }
    }
}

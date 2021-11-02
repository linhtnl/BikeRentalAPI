using AutoMapper;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using System;
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

        public UtilService(IUnitOfWork unitOfWork, IMapper mapper, IOwnerRepository ownerRepository) : base(unitOfWork, ownerRepository)
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

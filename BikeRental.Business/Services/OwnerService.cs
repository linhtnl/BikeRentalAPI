﻿using AutoMapper;
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
    public interface IOwnerService : IBaseService<Owner>
    {
        Task<OwnerViewModel> CreateNew(Owner ownerInfo);
        OwnerViewModel GetOwnerById(Guid id);
        Task<Owner> GetOwner(Guid id);
        Task<OwnerViewModel> GetByMail(string mail);
    }
    public class OwnerService : BaseService<Owner>, IOwnerService
    {
        private readonly IConfigurationProvider _mapper;
        public OwnerService(IUnitOfWork unitOfWork, IOwnerRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Owner> GetOwner(Guid id)
        {
            var owner = await GetAsync(id);
            return owner;
        }

        public OwnerViewModel GetOwnerById(Guid id)
        {
            return Get(x => x.Id.Equals(id)).ProjectTo<OwnerViewModel>(_mapper).FirstOrDefault();
        }

        public async Task<OwnerViewModel> GetByMail(string mail)
        {
            return Get().Where(tempOwner => tempOwner.Mail.Equals(mail)).ProjectTo<OwnerViewModel>(_mapper).FirstOrDefault();
        }

        public async Task<OwnerViewModel> CreateNew(Owner ownerInfo)
        {
            try
            {
                await CreateAsync(ownerInfo);

                return await GetByMail(ownerInfo.Mail);
            }
            catch
            {
                return null;
            }
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IMotorTypeService : IBaseService<MotorType>
    {
        Task<List<MotorTypeViewModel>> GetAll();
    }
    public class MotorTypeService : BaseService<MotorType>, IMotorTypeService
    {
        private readonly IConfigurationProvider _mapper;
        public MotorTypeService(IUnitOfWork unitOfWork, IMotorTypeRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<List<MotorTypeViewModel>> GetAll()
        {
            return await Get().ProjectTo<MotorTypeViewModel>(_mapper).ToListAsync();
        }
    }
}

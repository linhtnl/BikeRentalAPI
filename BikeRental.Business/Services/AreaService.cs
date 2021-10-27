
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.Utilities;
using BikeRental.Data.Enums;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IAreaService : IBaseService<Area>
    {
        Task<AreaViewModel> GetById(Guid id);
        Task<List<AreaViewModel>> GetAll(AreaViewModel model);
        Task<AreaViewModel> Update(Guid id,int postalCode, string name, string token);
        Task<AreaViewModel> Create(AreaCreateModel model, string token);
        Task<AreaViewModel> GetAreaByOwnerId(Guid id);
    }
    public class AreaService : BaseService<Area>, IAreaService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IOwnerService _ownerService;
        private readonly IConfiguration _configuration;

        public AreaService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper,
            IAreaRepository repository, 
            IOwnerService ownerService) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _ownerService = ownerService;
            _configuration = configuration;
        }

        public async Task<List<AreaViewModel>> GetAll(AreaViewModel model)
        {
            //customer&owner => Status = Available
            //Admin => all

            //customer&owner
            var areas = Get().ProjectTo<AreaViewModel>(_mapper)
                .DynamicFilter(model);
            var rs = await areas.ToListAsync();
            if(rs.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            return rs;
        }

        public async Task<AreaViewModel> Update(Guid id, int postalCode, string name, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            if (tokenModel.Role != (int)RoleConstants.Admin)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");
            
            Area area = Get(a => a.Id.Equals(id)).First();
            area.PostalCode = postalCode;
            area.Name = name;
            try
            {
                await UpdateAsync(area);
                var result = _mapper.CreateMapper().Map<AreaViewModel>(area);
                return result;
            }
            catch(Exception)
            {
                throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "Invalid Data");
            }
        }

        public async Task<AreaViewModel> Create(AreaCreateModel model, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            if (tokenModel.Role != (int)RoleConstants.Admin)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");

            var area = _mapper.CreateMapper().Map<Area>(model);
            try
            {
                await CreateAsync(area);
                var result = _mapper.CreateMapper().Map<AreaViewModel>(area);
                return result;
            }
            catch (Exception)
            {
                throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "Invalid Data");
            }    
        }

        public async Task<AreaViewModel> GetById(Guid id)
        {
            var area = await Get(a => a.Id.Equals(id)).ProjectTo<AreaViewModel>(_mapper).FirstOrDefaultAsync();
            if(area == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            return area;
        }

        public async Task<AreaViewModel> GetAreaByOwnerId(Guid id)
        {
            var owner = await _ownerService.GetOwnerById(id);
            if(owner == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            return await Get(a => a.Id.Equals(owner.AreaId)).ProjectTo<AreaViewModel>(_mapper).FirstOrDefaultAsync();
        }
    }
}

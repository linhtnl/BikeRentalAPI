using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IReportService : IBaseService<Report>
    {

        Task<List<ReportViewModel>> GetAll(string token);

        Task<ReportViewModel> GetById(Guid id,string token);

    }
    public class ReportService : BaseService<Report>, IReportService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;

        public ReportService(IUnitOfWork unitOfWork, IReportRepository repository,
            IMapper mapper, IConfiguration configuration) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;
        }

        public async Task<List<ReportViewModel>> GetAll(string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);
            int role = tokenModel.Role;
            if (role == (int)RoleConstants.Admin)
            {
                var result = await Get().ProjectTo<ReportViewModel>(_mapper).ToListAsync();
                if(result.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
                return result;
            }else throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");
        }

        public async Task<ReportViewModel> GetById(Guid id, string token)
        {
            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);
            int role = tokenModel.Role;
            if (role == (int)RoleConstants.Admin)
            {
                var result = await Get(f => f.Id.Equals(id)).ProjectTo<ReportViewModel>(_mapper).FirstOrDefaultAsync();
                if (result == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
                return result;
            }
            else throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");
        }
    }
}

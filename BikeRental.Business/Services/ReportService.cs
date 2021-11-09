using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IReportService : IBaseService<Report>
    {
        Task<List<ReportViewModel>> GetAll(string token);
        Task<ReportViewModel> GetById(Guid id, string token);
        Task<ReportViewModel> CreateNew(ReportCreateRequest request, string token);

    }
    public class ReportService : BaseService<Report>, IReportService
    {
        private readonly AutoMapper.IConfigurationProvider _mapper;
        private readonly IConfiguration _configuration;

        private readonly IBookingRepository _bookingRepository;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration,
            IReportRepository repository, 
            IBookingRepository bookingRepository
            ) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
            _configuration = configuration;

            _bookingRepository = bookingRepository;
        }

        public async Task<ReportViewModel> CreateNew(ReportCreateRequest request, string token)
        {
            int status = -1;

            TokenViewModel tokenModel = TokenService.ReadJWTTokenToModel(token, _configuration);

            Guid userId = tokenModel.Id;
            int role = tokenModel.Role;
            if (role == (int)ReportStatus.Customer || role == (int)ReportStatus.Owner)
                status = role;

            if (status == -1)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Your role cannot use this feature.");

            var targetBooking = await _bookingRepository.GetAsync(request.Id);
            if (targetBooking == null)
                throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "This booking is not found.");

            if ((status == (int)ReportStatus.Customer && userId != targetBooking.CustomerId) 
                    || (status == (int)ReportStatus.Owner && userId != targetBooking.OwnerId))
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "You just can report your own booking.");

            var targetReport = _mapper.CreateMapper().Map<Report>(request);
            targetReport.Status = status;

            await CreateAsync(targetReport);

            string roleInString = null;

            if (role == (int)ReportStatus.Customer)
                roleInString = "customer";
            else if (role == (int)ReportStatus.Owner)
                roleInString = "owner";

            TrackingReportUtil reportUtil = new TrackingReportUtil();
            await reportUtil.TrackNewReport(request.Id, new TrackingReportViewModel
            {
                Content = request.Content, 
                Role = roleInString
            });

            var result = _mapper.CreateMapper().Map<ReportViewModel>(targetReport);

            return await Task.Run(() => result);
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
            } else if (role == (int)RoleConstants.Customer || role == (int)RoleConstants.Owner)
            {
                var result = await Get(f => f.Id.Equals(id)).ProjectTo<ReportViewModel>(_mapper).FirstOrDefaultAsync();
                if (result == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
                else
                {
                    throw new ErrorResponse((int) HttpStatusCode.OK, "Found");
                }
            }
            else throw new ErrorResponse((int)HttpStatusCode.NotAcceptable, "This role cannot use this feature");
        }
    }
}

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface ICampaignService : IBaseService<Campaign>
    {
        Task<Campaign> CreateNew(CampaignCreateRequest campaign);
        Task<Campaign> Update(Guid id, CampaignUpdateRequest campaign);
        Task<bool> Delete(Guid id);
        Task<DynamicModelResponse<CampaignViewModel>> GetAll(CampaignViewModel model,int size, int pageNum);
        Task<CampaignViewModel> GetById(Guid id);
        List<CampaignViewModel> GetByAreaId(Guid areaId);
        List<CampaignViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate); // this method is used to get all campaign that start in the range time
        List<CampaignViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate); // this method is used to get all campaign that end in the range time
    }
    public class CampaignService : BaseService<Campaign>, ICampaignService
    {
        private readonly IConfigurationProvider _mapper;
        public CampaignService(IUnitOfWork unitOfWork, ICampaignRepository campaignRepository, IMapper mapper) : base(unitOfWork, campaignRepository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Campaign> CreateNew(CampaignCreateRequest model)
        {
            try
            {
                var campaign = _mapper.CreateMapper().Map<Campaign>(model);
                await CreateAsync(campaign);
                return campaign;
            }
            catch
            {
                throw new ErrorResponse((int)HttpStatusCode.InternalServerError, "Something went wrong.");
            }
        }

        //not yet
        public async Task<DynamicModelResponse<CampaignViewModel>> GetAll(CampaignViewModel model,int size, int pageNum)
        {
            //user token => userID => areaId => listCampaign base on areaId
            var campaigns = Get().ProjectTo<CampaignViewModel>(_mapper)
                .DynamicFilter(model)
                .PagingIQueryable(pageNum, size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
            if (campaigns.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var rs = new DynamicModelResponse<CampaignViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = pageNum,
                    Size = size,
                    Total = campaigns.Item1
                },
                Data = await campaigns.Item2.ToListAsync()
            };
            return rs;
        }
        


        public async Task<CampaignViewModel> GetById(Guid id)
        {
            return await Get().Where(tempCampaign => tempCampaign.Id.Equals(id))
                .ProjectTo<CampaignViewModel>(_mapper)
                .FirstOrDefaultAsync();
        }

        public List<CampaignViewModel> GetByAreaId(Guid areaId)
        {
            return Get().Where(tempCampain => tempCampain.AreaId.Equals(areaId))
                .ProjectTo<CampaignViewModel>(_mapper)
                .ToList();
        }

        public List<CampaignViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate)
        {
            return Get().Where(tempCampaign => tempCampaign.StartingDate >= startDate && tempCampaign.StartingDate <= endDate)
                .ProjectTo<CampaignViewModel>(_mapper)
                .ToList();
        }

        public List<CampaignViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate)
        {
            return Get().Where(tempCampaign => tempCampaign.ExpiredDate >= startDate && tempCampaign.ExpiredDate <= endDate)
                .ProjectTo<CampaignViewModel>(_mapper)
                .ToList();
        }

        public async Task<bool> Delete(Guid id)
        {
            var campaign = Get(c => c.Id.Equals(id)).FirstOrDefault();
            campaign.Status = (int)CampaignStatus.Unavailable;
            try
            {
                await UpdateAsync(campaign);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Campaign> Update(Guid id, CampaignUpdateRequest request)
        {
            var campaign = await GetAsync(id);
            if (campaign == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Bike not found");
            var updateBike = _mapper.CreateMapper().Map(request, campaign);
            await UpdateAsync(updateBike);
            return updateBike;
        }
    }
}

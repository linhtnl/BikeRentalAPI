using AutoMapper;
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
    public interface ICampaignService : IBaseService<Campaign>
    {
        Task<bool> CreateNew(CampaignViewModel campaign);
        List<CampaignViewModel> GetAll();
        CampaignViewModel GetById(Guid id);
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

        public async Task<bool> CreateNew(CampaignViewModel campaignRequest)
        {
            try
            {
                Campaign campaign = new Campaign(campaignRequest);

                await CreateAsync(campaign);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<CampaignViewModel> GetAll()
        {
            return Get().Where(tempCampaign => (bool)tempCampaign.IsHappening)
                .ProjectTo<CampaignViewModel>(_mapper)
                .ToList();
        }

        public CampaignViewModel GetById(Guid id)
        {
            return Get().Where(tempCampaign => tempCampaign.Id.Equals(id))
                .ProjectTo<CampaignViewModel>(_mapper)
                .FirstOrDefault();
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
    }
}

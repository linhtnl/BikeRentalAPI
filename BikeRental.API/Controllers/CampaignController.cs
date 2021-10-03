using BikeRental.API.Models.Request;
using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v1.0/campaigns")]
    [ApiController]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpPost]
        public Task<bool> CreateNew([FromBody] CampaignViewModel campainRequest)
        {
            return _campaignService.CreateNew(campainRequest);
        }

        [HttpGet]
        public List<CampaignViewModel> GetAll()
        {
            return _campaignService.GetAll();
        }

        [HttpGet("id/{id}")]
        public CampaignViewModel GetById(Guid id)
        {
            return _campaignService.GetById(id);
        }

        [HttpGet("areaId/{areaId}")]
        public List<CampaignViewModel> GetByAreaId(Guid areaId)
        {
            return _campaignService.GetByAreaId(areaId);
        }

        [HttpGet("inRangeDate/start")]
        public List<CampaignViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _campaignService.GetStartInRangeDate(startDate, endDate);
        }

        [HttpGet("inRangeDate/end")]
        public List<CampaignViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _campaignService.GetEndInRangeDate(startDate, endDate);
        }
    }
}

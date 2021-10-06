using BikeRental.API.Models.Request;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
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
    [Route("api/v{version:apiVersion}/campaigns")]
    [ApiController]
    [ApiVersion("1")]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> CreateNew([FromBody] CampaignCreateRequest campainRequest)
        {
            var campaign = await _campaignService.CreateNew(campainRequest);
            return campaign != null
                ? await Task.Run(() => Ok(campaign))
                : await Task.Run(() => StatusCode(ResponseStatusConstants.FORBIDDEN));
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] CampaignViewModel model, int page = CommonConstants.DefaultPage)
        {
            return Ok(await _campaignService.GetAll(model, page));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _campaignService.GetById(id));
        }
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            return Ok(await _campaignService.Delete(id));
        }

        /*     [HttpGet("areaId/{areaId}")]
             public List<CampaignViewModel> GetByAreaId(Guid areaId)
             {
                 return _campaignService.GetByAreaId(areaId);
             }*/

        /*[HttpGet("inRangeDate/start")]
        public List<CampaignViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _campaignService.GetStartInRangeDate(startDate, endDate);
        }

        [HttpGet("inRangeDate/end")]
        public List<CampaignViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _campaignService.GetEndInRangeDate(startDate, endDate);
        }*/
    }
}

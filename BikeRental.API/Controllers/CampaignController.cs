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
    [ApiVersion("2")]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
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
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> CreateNew([FromBody] CampaignCreateRequest campainRequest)
        {
            var campaign = await _campaignService.CreateNew(campainRequest);
            return campaign != null
                ? await Task.Run(() => Ok(campaign))
                : await Task.Run(() => StatusCode(ResponseStatusConstants.FORBIDDEN));
        }
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CampaignUpdateRequest request)
        {
            return Ok(await _campaignService.Update(id, request));
        }
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            return Ok(await _campaignService.Delete(id));
        }

        [HttpGet("areaId/{areaId}")]
        [MapToApiVersion("2")]
        public List<CampaignViewModel> GetByAreaId(Guid areaId)
        {
            return _campaignService.GetByAreaId(areaId);
        }

        [HttpGet("inRangeDate/start")]
        [MapToApiVersion("2")]
        public List<CampaignViewModel> GetStartInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _campaignService.GetStartInRangeDate(startDate, endDate);
        }

        [HttpGet("inRangeDate/end")]
        [MapToApiVersion("2")]
        public List<CampaignViewModel> GetEndInRangeDate(DateTime startDate, DateTime endDate)
        {
            return _campaignService.GetEndInRangeDate(startDate, endDate);
        }
    }
}

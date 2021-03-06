using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Get([FromQuery] CampaignViewModel model, int size, int page = CommonConstants.DefaultPage)
        {
            return Ok(await _campaignService.GetAll(model, size, page));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _campaignService.GetById(id));
        }

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> CreateNew([FromBody] CampaignCreateRequest campainRequest)
        {
            var campaign = await _campaignService.CreateNew(campainRequest);

            return await Task.Run(() => Ok(campaign));
        }

        [Authorize]
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CampaignUpdateRequest request)
        {
            return Ok(await _campaignService.Update(id, request));
        }

        [Authorize]
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            return Ok(await _campaignService.Delete(id));
        }

        [HttpGet("areaId/{areaId}")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetByAreaId(Guid areaId)
        {
            var result = await _campaignService.GetByAreaId(areaId);

            return await Task.Run(() => Ok(result));
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

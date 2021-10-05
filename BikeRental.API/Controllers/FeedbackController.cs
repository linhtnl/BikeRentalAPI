using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/feedbacks")]
    [ApiController]
    [ApiVersion("1")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create(FeedbackCreateRequest request)
        {
            return Ok(await _feedbackService.Create(request));
        }

        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(Guid id,FeedbackCreateRequest request)
        {
            return Ok(await _feedbackService.Update(id,request));
        }
    }
}

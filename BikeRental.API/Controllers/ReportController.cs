using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/reports")]
    [ApiController]
    [ApiVersion("1")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IConfiguration _configuration;

        public ReportController(IReportService reportService, IConfiguration configuration)
        {
            _reportService = reportService;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            return Ok(await _reportService.GetById(id,token));
        }

        [Authorize]
        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAll()
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _reportService.GetAll(token));
        }

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create(ReportCreateRequest request)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            var result = await _reportService.CreateNew(request, token);

            return await Task.Run(() => Ok(result));
        }
    }
}

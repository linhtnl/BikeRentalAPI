using BikeRental.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/motorTypes")]
    [ApiController]
    [ApiVersion("1")]
    public class MotorTypeController : ControllerBase
    {
        private readonly IMotorTypeService _motorService;
        public MotorTypeController(IMotorTypeService motorTypeService)
        {
            _motorService = motorTypeService;
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _motorService.GetAll());
        }
    }
}

using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/areas")]
    [ApiController]
    [ApiVersion("1")]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] AreaViewModel model)
        {
            return Ok(await _areaService.GetAll(model));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetArea(Guid id)
        {
            return Ok(await _areaService.GetById(id));
        }

        [HttpGet("areaByOwnerId")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAreaByOwnerId(Guid id)
        {
            return Ok(await _areaService.GetAreaByOwnerId(id));
        }

        [Authorize]
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update([FromQuery]Guid id , [FromQuery] int postalCode, [FromQuery] string name)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            return  Ok(await _areaService.Update(id, postalCode, name, token));
        }

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create([FromBody]AreaCreateModel model)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            return Ok(await _areaService.Create(model, token));
        }
    }
}

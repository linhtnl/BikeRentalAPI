
using BikeRental.Business.Services;
using BikeRental.Business.Constants;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BikeRental.Data.Responses;
using System.Net;

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
        //[ProducesResponseType(typeof(DynamicModelResponse<AreaViewModel>), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        //[ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] AreaViewModel model, int page = CommonConstants.DefaultPage)
        {
            return Ok(await _areaService.GetAll(model,page));
        }
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetArea(Guid id)
        {
            return Ok(await _areaService.GetById(id));
        }
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<Area> Update(Guid id , int postalCode, string name)
        {
                return await _areaService.Update(id, postalCode, name);
        }
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<Area> Create(AreaCreateModel model)
        {
            return await _areaService.Create(model);
        }
    }
}

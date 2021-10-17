using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/bookings")]
    [ApiController]
    [ApiVersion("1")]
   
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet()]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _bookingService.GetAll());
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _bookingService.GetById(id));
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize]
        public async Task<IActionResult> Create([FromHeader] string token, [FromBody] BookingCreateRequest request)
        {
            var bookingResult = await _bookingService.CreateNew(token, request);

            return await Task.Run(() => Ok(bookingResult));
        }

        [HttpPut]
        [MapToApiVersion("1")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus([FromHeader] string token, [FromBody] BookingUpdateStatusRequest request)
        {
            var bookingResult = await _bookingService.UpdateStatus(token, request);

            return await Task.Run(() => Ok(bookingResult));
        }
    }
}

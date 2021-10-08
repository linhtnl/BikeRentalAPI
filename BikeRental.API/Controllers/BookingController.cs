using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
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
    [ApiVersion("2")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet()]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _bookingService.GetAll());
        }

        [HttpGet("{id}")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            return Ok(await _bookingService.GetById(id));
        }

    }
}

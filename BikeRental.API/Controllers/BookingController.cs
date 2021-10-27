using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Create([FromBody] BookingCreateRequest request)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            var bookingResult = await _bookingService.CreateNew(token, request);

            return await Task.Run(() => Ok(bookingResult));
        }

        [Authorize]
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> UpdateStatus([FromBody] BookingUpdateStatusRequest request)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            var bookingResult = await _bookingService.UpdateStatus(token, request);

            return await Task.Run(() => Ok(bookingResult));
        }
    }
}

using BikeRental.Business.Constants;
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

        [Authorize]
        [HttpGet()]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAll(int status, int size, int pageNum = CommonConstants.DefaultPage)
        {
            string token = null;

            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            };

            return Ok(await _bookingService.GetAll(token,status, size,pageNum));
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
            return Ok(await _bookingService.GetBookingDetailById(id, token));
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

        [HttpGet("saveEvidence")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> SaveEvidence(Guid bookingId, string path)
        {
            var result = await _bookingService.SaveBookingEvidence(bookingId, path);

            return await Task.Run(() => Ok(result));
        }

        [Authorize]
        [HttpGet("SendConfirmedNoti")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> SendConfirmedNoti(Guid bookingId)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _bookingService.SendConfirmNoti(token, bookingId));
        }
    }
}

using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Business.Utilities;
using BikeRental.Data.Models;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/owners")]
    [ApiController]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class OwnerController : Controller
    {
        private readonly IOwnerService _ownerService;
        private readonly IConfiguration _configuration;
        public OwnerController(IOwnerService ownerService, IConfiguration configuration)
        {
            _ownerService = ownerService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Login([FromBody] OwnerLoginRequest request)
        {
            string token = await _ownerService.Login(request, _configuration);

            return await Task.Run(() => Ok(token));
        }

        [HttpPost("register")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Register([FromBody] OwnerRegisterRequest request)
        {
            string token = await _ownerService.Register(request, _configuration);

            return await Task.Run(() => Ok(token));
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Get([FromQuery] OwnerRatingViewModel model, int filterOption ,int size, int page = CommonConstants.DefaultPage)
        {
            return Ok(await _ownerService.GetAll(model,filterOption, size, page));
        }

        [HttpGet("find")]
        [MapToApiVersion("2")]
        [Authorize]
        public async Task<IActionResult> Get(Guid areaId, Guid typeId, DateTime dateRent, DateTime dateReturn, int? timeRent, double totalPrice, string address, string customerLocation)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            return Ok(await _ownerService.GetListOwnerByAreaIdAndTypeId(areaId, typeId, token, dateRent, dateReturn, timeRent,totalPrice,address, customerLocation));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _ownerService.GetOwnerById(id));
        }

        [Authorize]
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            return Ok(await _ownerService.Delete(id));
        }

        [HttpPost("TestRead")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> Test(string token)
        {
            var result = TokenService.ReadJWTTokenToModel(token, _configuration);

            return await Task.Run(() => Ok(result));
        }

        [HttpPost("TestGenerate")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> TestGenerate(OwnerViewModel ownerInfo)
        {
            var result = TokenService.GenerateOwnerJWTWebToken(ownerInfo, _configuration);

            return await Task.Run(() => Ok(result));
        }

        [HttpGet("TestWriteFirebase")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> TestWriteFirebase(Guid id)
        {
            TrackingOnlineUtil trackingOnlineUtil = new TrackingOnlineUtil();

            var result = await trackingOnlineUtil.TrackNewUserLogin(id);

            return await Task.Run(() => Ok(result));
        }

        [HttpGet("TestGetExpiredTime")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> TestGetExpiredTime(Guid id)
        {
            TrackingOnlineUtil trackingOnlineUtil = new TrackingOnlineUtil();

            var result = await trackingOnlineUtil.GetUserExpiredTime(id);

            return await Task.Run(() => Ok(result));
        }

        [Authorize]
        [HttpGet("testAuthen")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> TestAuthen()
        {

            return await Task.Run(() => Ok(true));
        }

        /*[HttpGet("testDistance")]
        [MapToApiVersion("2")]
        [Authorize]
        public async Task<IActionResult> TestDistance(Guid areaId, Guid typeId, string customerLocation, DateTime dateRent, DateTime dateReturn)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }
            List<OwnerByAreaViewModel> suitableOwners = await _ownerService.GetListOwnerByAreaIdAndTypeId(areaId, typeId,token, dateRent, dateReturn);

            var result = await DistanceUtil.OrderByDistance(suitableOwners, customerLocation);

            return await Task.Run(() => Ok(result));
        }*/

        [HttpGet("testSendNoti")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> SendNoti(Guid ownerId, [FromQuery]CustomerRequestModel request)
        {
            return Ok(await _ownerService.SendNoti(ownerId,request));
        }

        [HttpGet("testGetTrackingBooking")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetTrackingBookingByDate(Guid ownerId, DateTime date)
        {
            return Ok(await TrackingBookingUtil.GetTrackingBookingByDate(ownerId, date));
        }

        [HttpGet("testUpdateTrackingBooking")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> TestUpdateTrackingBooking(Guid ownerId, DateTime date)
        {
            return Ok(await TrackingBookingUtil.UpdateTrackingBooking(ownerId, date));
        }

        [HttpGet("testUpdateTrackingBookingDays")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> TestUpdateTrackingBookingDays(Guid ownerId, DateTime dayRent, DateTime dayReturn)
        {
            return Ok(await TrackingBookingUtil.UpdateTrackingBooking(ownerId, dayRent, dayReturn));
        }

        [HttpGet("testGetOwnerRegistrationId")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> TestGetOwnerRegistrationId(Guid ownerId)
        {
            return Ok(await TrackingRegistrationIdUtil.GetOwnerRegistrationId(ownerId));
        }
    }
}

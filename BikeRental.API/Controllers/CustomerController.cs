using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/customers")]
    [ApiController]
    [ApiVersion("1")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IConfiguration _configuration;

        public CustomerController(ICustomerService customerService, IConfiguration configuration)
        {
            _customerService = customerService;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _customerService.GetCustomerById(id));
        }

        [HttpPost("login")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginRequest request)
        {
            var token = await _customerService.Login(request.PhoneNumber, _configuration);

            return await Task.Run(() => Ok(token));
        }

        [HttpPost("register")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Register([FromBody] CustomerCreateRequest request)
        {
            var token = await _customerService.Register(request, _configuration);

            return await Task.Run(() => Ok(token));
        }

        [Authorize]
        [HttpPut]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Update(CustomerUpdateRequest request)
        {
            var updatedCustomer = await _customerService.UpdateCustomer(request);

            return await Task.Run(() => Ok(updatedCustomer));
        }

        [Authorize]
        [HttpPut("unban")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Unban([FromBody]Guid id)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            var result = await _customerService.UnbanCustomer(id, token);

            return await Task.Run(() => Ok(result));
        }

        [Authorize]
        [HttpDelete]
        [MapToApiVersion("1")]
        public async Task<IActionResult> Delete([FromBody]Guid id)
        {
            string token = null;
            if (Request.Headers["Authorization"].Count > 0)
            {
                token = Request.Headers["Authorization"];
            }

            var result = await _customerService.DeleteCustomer(id, token);

            return await Task.Run(() => Ok(result));
        }
    }
}

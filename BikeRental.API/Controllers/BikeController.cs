using BikeRental.Business.Services;

using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v1.0/bikes")]
    [ApiController]
    public class BikeController : ControllerBase
    {
        private readonly IBikeService _bikeService;
        public BikeController(IBikeService bikeService)
        {
            _bikeService = bikeService;
        }

        [HttpGet]        
        public List<BikeViewModel> Get(int pageNum)
        {
            return _bikeService.GetAll(pageNum);
        }
    }
}

using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Enums;
using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/wallets")]
    [ApiController]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [Authorize]
        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> CreateNew([FromBody] WalletCreateRequest walletRequest)
        {
            var wallet = _walletService.CreateNew(walletRequest);

            return await Task.Run(() => Ok(wallet));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public WalletViewModel GetById(Guid id)
        {
            return _walletService.GetById(id);
        }

        [HttpGet("bankId/{bankId}")]
        [MapToApiVersion("2")]
        public WalletViewModel GetByBankId(string bankId)
        {
            return _walletService.GetByBankId(bankId);
        }

        [HttpGet("momoId/{momoId}")]
        [MapToApiVersion("2")]
        public WalletViewModel GetByMomoId(string momoId)
        {
            return _walletService.GetByMomoId(momoId);
        }

        [HttpGet("transactionHistory/{id}")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetTransactionHistory(Guid id, int? filterOption, bool action, int size, int pageNum = CommonConstants.DefaultPage)
        {
            var result = await _walletService.GetTransactionHistory(id, action, size, pageNum, filterOption);

            return await Task.Run(() => Ok(result)); // this line need to handle the null case, the null case occur when filterOption is not supported yet
        }

        [HttpPut("depositAmount")] // this method must be implement checking verifyRequestToken in the header before action (login methods havent been implemented yet)
        [MapToApiVersion("2")]
        public async Task<bool> DepositAmount([FromBody] WalletRequest requestData)
        {
            return await _walletService
                .UpdateAmount(requestData.WalletId, requestData.Amount, (int) WalletStatus.DEPOSIT, requestData.BookingId);             
        }

        [HttpPut("decreaseAmount")] // this method must be implement checking verifyRequestToken in the header before action (login methods havent been implemented yet)
        [MapToApiVersion("2")]
        public async Task<bool> DecreaseAmount([FromBody] WalletRequest requestData)
        {          
            return await _walletService
                .UpdateAmount(requestData.WalletId, requestData.Amount, (int)WalletStatus.DECREASE, requestData.BookingId);    
        }
    }
}

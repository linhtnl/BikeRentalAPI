using BikeRental.API.Models.Request;
using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Enums;
using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    [Route("api/v{version:apiVersion}/wallets")]
    [ApiController]
    [ApiVersion("1")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        // this should be PUT or POST to make sure user can watch another's wallet
        [HttpGet("id/{id}")]
        [MapToApiVersion("1")]
        public WalletViewModel GetById(string id)
        {
            Guid guid = Guid.Parse(id);
            return _walletService.GetById(guid);
        }

        [HttpGet("bankId/{bankId}")]
        [MapToApiVersion("1")]
        public WalletViewModel GetByBankId(string bankId)
        {
            return _walletService.GetByBankId(bankId);
        }

        [HttpGet("momoId/{momoId}")]
        [MapToApiVersion("1")]
        public WalletViewModel GetByMomoId(string momoId)
        {
            return _walletService.GetByMomoId(momoId);
        }

        [HttpGet("transactionHistory/{id}")]
        [MapToApiVersion("1")]
        public List<TransactionHistoryViewModel> GetTransactionHistory(string id, int pageNum, int? filterOption)
        {
            Guid guid = Guid.Parse(id);
            return _walletService.GetTransactionHistory(guid, pageNum, filterOption); // this line need to handle the null case, the null case occur when filterOption is not supported yet
        }

        [HttpPut("depositAmount")] // this method must be implement checking verifyRequestToken in the header before action (login methods havent been implemented yet)
        [MapToApiVersion("1")]
        public async Task<bool> DepositAmount([FromBody] WalletRequest requestData)
        {
                return await _walletService.UpdateAmount(requestData.Id, requestData.Amount, (int) WalletStatus.DEPOSIT);             
        }

        [HttpPut("decreaseAmount")] // this method must be implement checking verifyRequestToken in the header before action (login methods havent been implemented yet)
        [MapToApiVersion("1")]
        public async Task<bool> DecreaseAmount([FromBody] WalletRequest requestData)
        {          
               return await _walletService.UpdateAmount(requestData.Id, requestData.Amount, (int)WalletStatus.DECREASE);    
        }
    }
}

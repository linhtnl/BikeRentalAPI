using BikeRental.API.Models.Request;
using BikeRental.Business.Constants;
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
    // all methods of this controller should be implement in PUT or POST Http Method,
    // to keep user personal information secret
    // fix later (delete this comment if this has fixed already)
    [Route("api/v1.0/wallets")]
    [ApiController]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        // this should be PUT or POST to make sure user can watch another's wallet
        [HttpGet("id/{id}")]
        public WalletViewModel GetById(string id)
        {
            Guid guid = Guid.Parse(id);
            return _walletService.GetById(guid);
        }

        [HttpGet("bankId/{bankId}")]
        public WalletViewModel GetByBankId(string bankId)
        {
            return _walletService.GetByBankId(bankId);
        }

        [HttpGet("momoId/{momoId}")]
        public WalletViewModel GetByMomoId(string momoId)
        {
            return _walletService.GetByMomoId(momoId);
        }

        [HttpGet("transactionHistory/{id}")]
        public List<TransactionHistoryViewModel> GetTransactionHistory(string id, int pageNum, int? filterOption)
        {
            Guid guid = Guid.Parse(id);
            return _walletService.GetTransactionHistory(guid, pageNum, filterOption); // this line need to handle the null case, the null case occur when filterOption is not supported yet
        }

        [HttpPut("depositAmount")] // this method must be implement checking verifyRequestToken in the header before action (login methods havent been implemented yet)
        public async Task<bool> DepositAmount([FromBody] WalletRequest requestData)
        {
                return await _walletService.UpdateAmount(requestData.Id, requestData.Amount, (int) WalletStatus.DEPOSIT);             
        }

        [HttpPut("decreaseAmount")] // this method must be implement checking verifyRequestToken in the header before action (login methods havent been implemented yet)
        public async Task<bool> DecreaseAmount([FromBody] WalletRequest requestData)
        {          
               return await _walletService.UpdateAmount(requestData.Id, requestData.Amount, (int)WalletStatus.DECREASE);    
        }
    }
}

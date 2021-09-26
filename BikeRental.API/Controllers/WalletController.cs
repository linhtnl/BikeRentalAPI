using BikeRental.Business.Services;
using BikeRental.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Controllers
{
    // all methods of this controller should be implement in PUT or POST Http Method,
    // to keep user personal information secret
    // fix later (delete this comment if this has fixed already)
    [Route("api/wallet")]
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
        public WalletViewModel GetTransactionHistory(string id)
        {
            Guid guid = Guid.Parse(id);
            throw new NotImplementedException();
        }
    }
}

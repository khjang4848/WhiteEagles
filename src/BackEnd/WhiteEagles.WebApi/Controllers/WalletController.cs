namespace WhiteEagles.WebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Data.ViewModels;
    using Filters;
    using Data.Services;
    using Common;

    [ApiController]
    [Route("[controller]")]
    public class WalletController : CustomControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IMerchantInfoService _merchantInfoService;
        private readonly IConfiguration _config;
        private readonly IPGInquiryService _pgInquiryService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, 
            IMerchantInfoService merchantInfoService,
            IConfiguration config, ILogger<WalletController> logger,
            IPGInquiryService pgInquiryService)
        {
            _walletService = walletService ??
                             throw new ArgumentNullException(nameof(walletService));
            _merchantInfoService = merchantInfoService ??
                             throw new ArgumentNullException(nameof(walletService));
            _pgInquiryService = pgInquiryService ??
                                throw new ArgumentNullException(nameof(pgInquiryService));
            _config = config;
            _logger = logger;
        }


        [HttpGet]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("SelectAvailableLimit")]
        public async Task<IActionResult> SelectAvailableLimit(string mid)
        {
            var result = await _walletService.SelectAvailableLimit(mid);
            var merchantInfo = await _pgInquiryService.PGInquiryMerchantInfoAsync(mid);

            result.MerchantName = merchantInfo?.merchantName;
            result.MerchantNo = merchantInfo?.businessNo;

            return Ok(result);
        }


        [HttpGet]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("SelectAvailableLimitAll")]
        public async Task<IActionResult> SelectAvailableLimitAll(string mid)
        {
            var result = await _walletService.SelectAvailableLimitAll(mid);
            var info = new List<LimitListViewModel>();
            var preMonth = DateTime.Now.AddMonths(-1).ToString("yyyyMM");

            foreach (var item in result)
            {
                var merchantInfo = await _pgInquiryService.PGInquiryMerchantInfoAsync(item.MerchantId);
                var salesInfo = await _pgInquiryService.PGInquirySalesSumInfo(mid, preMonth);

                info.Add(new LimitListViewModel
                {
                    MerchantId = item.MerchantId,
                    MerchantNo = merchantInfo?.businessNo,
                    MerchantName = merchantInfo?.merchantName,
                    TelNo = merchantInfo?.merchantTelNo,
                    SuspenseReceipts = item.SuspenseReceipts,
                    PreSalesAmount = salesInfo,
                    TransferLimit = 0,
                    AvailableLimit = item.SuspenseReceipts + item.DepositSum - item.WithdrawalSum
                });
            }

            return Ok(info);
        }

        [HttpPost]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("LimitRegister")]
        public async Task<IActionResult> LimitRegister(LimitRegisterInfo info)
        {
            var merchantInfo = await _merchantInfoService.SelectMerchantInfo(info.MerchantId);

            if (merchantInfo == null)
            {
                return BadRequest("존재하는 않는 MerchantID입니다");
            }

            var result = _walletService.RegisterLimit(info);
            return Ok(result);
        }

    }
}

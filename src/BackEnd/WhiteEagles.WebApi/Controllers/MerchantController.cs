namespace WhiteEagles.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Data.Models;
    using Data.Services;
    using Common;
    using Filters;
    using Data.ViewModels;

    [ApiController]
    [Route("[controller]")]
    public class MerchantController : CustomControllerBase
    {
        private readonly IMerchantInfoService _merchantInfoService;
        private readonly IWalletService _walletService;
        private readonly IConfiguration _config;
        private readonly ILogger<MerchantController> _logger;

        public MerchantController(IMerchantInfoService merchantInfoService,
            IWalletService walletService,
            IConfiguration config,
            ILogger<MerchantController> logger)
        {
            _merchantInfoService = merchantInfoService ??
                            throw new ArgumentNullException(nameof(merchantInfoService));
            _walletService = walletService ??
                            throw new ArgumentNullException(nameof(walletService));
            _config = config;
            _logger = logger;
        }


        [HttpPost]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("Insert")]
        public async Task<IActionResult> InsertMerchant(MerchantInfo model)
        {

            var merchantInfo = await _merchantInfoService.SelectMerchantInfo(model.MerchantId);

            if (merchantInfo != null)
            {
                return BadRequest("이미 존재하는 MerchantID입니다");
            }

            var merchantCodeCount = await _merchantInfoService.SelectMerchantCount();

            if (merchantCodeCount == 0)
            {
                model.MerchantCode = "TR_0001";
            }
            else
            {
                var source = await _merchantInfoService.SelectMerchantCode();
                model.MerchantCode = ConvertMerchantCode(source);
            }

            return Ok(await _merchantInfoService.InsertMerchantInfo(model));
        }

        [HttpPost]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("Update")]
        public async Task<IActionResult> UpdateMerchant(MerchantInfo model)
        {
            var merchantInfo = await _merchantInfoService.SelectMerchantInfo(model.MerchantId);

            if (merchantInfo == null)
            {
                return BadRequest("존재하는 않는 MerchantID입니다");
            }

            return Ok(await _merchantInfoService.UpdateMerchantInfo(model));

        }

        [HttpGet]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [Route("Select")]
        public async Task<IActionResult> SelectMerchant(string mid)
            => Ok(await _merchantInfoService.SelectMerchantInfo(mid));

        [HttpPost]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("RegisterLimit")]
        public async Task<IActionResult> RegisterLimit(LimitRegisterInfo info)
        {
            await _walletService.RegisterLimit(info);
            return Ok();
        }

        private string ConvertMerchantCode(string source)
        {
            var result1 = source.Substring(3);
            var result2 = Convert.ToInt32(result1) + 1;
            return $"TR_{result2.ToString().PadLeft(4, '0')}";
        }


    }
}

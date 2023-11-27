namespace WhiteEagles.WebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    using Filters;
    using Data.Services;
    using Common;

    [ApiController]
    [Route("[controller]")]
    public class CalculateController : CustomControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<CalculateController> _logger;
        private readonly ICalculateService _calculateService;
        private readonly IPGInquiryService _pgInquiryService;

        public CalculateController(IConfiguration config,
            ILogger<CalculateController> logger,
            ICalculateService calculateService, IPGInquiryService pgInquiryService)
        {
            _calculateService = calculateService
                                ?? throw new ArgumentNullException(nameof(calculateService));
            _pgInquiryService = pgInquiryService
                                ?? throw new ArgumentNullException(nameof(pgInquiryService));

            _config = config;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [Route("SelectCalculateInfo")]
        public async Task<IActionResult> SelectCalculateInfo(string startDate, 
            string endDate, string mid)
        {
            var result = await _calculateService
                .SelectCalculateInfo(startDate, endDate, mid);


            foreach (var item in result)
            {
                var merchantInfo = await _pgInquiryService.PGInquiryMerchantInfoAsync(item.MerchantId);

                item.MerchantName = merchantInfo?.merchantName;
                item.MerchantNo = merchantInfo?.businessNo;
                item.TelNo = merchantInfo?.merchantTelNo;
            }

            return Ok(result);
        }

        [HttpGet]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [Route("SelectCalculateInfoAll")]
        public async Task<IActionResult> SelectCalculateInfoAll(string startDate,
            string endDate, string mid)
        {
            var result = await _calculateService.SelectCalculateInfoAll(startDate, 
                endDate, mid);

            foreach (var item in result)
            {
                var merchantInfo = await _pgInquiryService.PGInquiryMerchantInfoAsync(item.MerchantId);

                item.MerchantName = merchantInfo?.merchantName;
                item.MerchantNo = merchantInfo?.businessNo;
                item.TelNo = merchantInfo?.merchantTelNo;
            }

            return Ok(result);
        }
        
    }
}

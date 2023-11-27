namespace WhiteEagles.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Mvc;
    using ClosedXML.Excel;
    using ClosedXML.Extensions;
    using AutoMapper.Internal;

    using Data.Services;
    using Data.ViewModels;
    using Filters;
    using Common;

    [ApiController]
    [Route("[controller]")]
    public class TransferController : CustomControllerBase
    {
        private readonly ITransferService _transferService;
        private readonly IWalletService _walletService;
        private readonly IConfiguration _config;
        private readonly ILogger<TransferController> _logger;

        public TransferController(ITransferService transferService,
            IWalletService walletService,
            IConfiguration config, ILogger<TransferController> logger)
        {
            _transferService = transferService
                ?? throw new ArgumentNullException(nameof(transferService));
            _walletService = walletService
                             ?? throw new ArgumentNullException(nameof(walletService));
            _config = config;
            _logger = logger;
        }


        [HttpPost]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("WalletHistory")]
        public async Task<IActionResult> SelectWalletHistory(WalletHistoryRequestView info)
        {
            var result = await _walletService.SelectWalletHistory(info);
            return Ok(result);
        }

        [HttpGet]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("WalletHistoryExcel")]
        public async Task<IActionResult> SelectWalletExcel(string startDate, string endDate,
            string mid, string remiName, string inAccountNo, string bankCode,
            string transferStatus)
        {
            var requestInfo = new WalletHistoryRequestView()
            {
                StartDate = startDate,
                EndDate = endDate,
                Mid = mid,
                RemiName = remiName,
                InAccountNo = inAccountNo,
                BankCode = bankCode,
                TransferStatus = transferStatus
            };

            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Sheet1");

            var result = await _walletService.SelectWalletHistoryAll(requestInfo);

            var index = 1;

            result.ForAll(x =>
            {
                ws.Cell(index, 1).Value = x.MerchantName;
                ws.Cell(index, 2).Value = x.MerchantId;
                ws.Cell(index, 3).Value = x.BankName;
                ws.Cell(index, 4).Value = x.AccountName;
                ws.Cell(index, 5).Value = x.AccountNo;
                ws.Cell(index, 6).Value = x.SendDateTime;
                ws.Cell(index, 7).Value = x.Amount;
                ws.Cell(index, 8).Value = x.ResultMessage;
                index++;
            });


            var response = wb.Deliver("excelfile.xlsx");

            return new FileContentResult(await response.Content.ReadAsByteArrayAsync(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "XXXName.xlsx"
            };
        }



        [HttpPost]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("WalletHistoryCount")]
        public async Task<IActionResult> SelectWalletHistoryCount(WalletHistoryRequestView info)
            => Ok(await _walletService.SelectWalletHistoryCount(info));


    }
}

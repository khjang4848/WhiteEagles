namespace WhiteEagles.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Data.Models;
    using Data.Services;
    using Data.ViewModels;
    using Filters;
    using Common;

    [ApiController]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        private readonly WorkData.TxTable _txTable;
        private readonly IAccountInquiryService _accountInquiryService;
        private readonly ITransferService _transferService;
        private readonly IMerchantInfoService _merchantInfoService;
        private readonly IWalletService _walletService;
        private readonly IConfiguration _config;
        private readonly ILogger<UserController> _logger;


        public UserController(
            WorkData.TxTable txTable,
            IAccountInquiryService accountInquiry,
            ITransferService transferService,
            IMerchantInfoService merchantInfoService,
            IWalletService walletService,
            IConfiguration config,
            ILogger<UserController> logger)
        {
            _txTable = txTable ??
                throw new ArgumentNullException(nameof(accountInquiry));

            _accountInquiryService = accountInquiry ??
                throw new ArgumentNullException(nameof(accountInquiry));

            _transferService = transferService ??
                throw new ArgumentNullException(nameof(transferService));

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
        [Route("AccountInquiry")]
        public async Task<IActionResult> AccountInquiry(AccountInquiryViewModel model)
        {
            var result = await _accountInquiryService.AccountInquiryAsync(model);

            var info = new AccountResponseViewModel
            {
                ResultCode = result.ResultCode,
                ResultMessage = result.ResultMessage,
                AccountName = result.AccountName?[0].AccountName
            };


            return Ok(info);
        }

        #region 출금 처리 관련 모듈 :  : YKLEE
        [HttpPost]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [ModelValidation]
        [Route("Withdrawal")]
        public async Task<IActionResult> Withdrawal(WithdrawalViewModel model)
        {
            // model.AccountName : 의 경우 AccountInquiry를 통해서 확인 하고 출금시에는 사용하지 않는 필드임

            // EF.Core 사용을 하지 않고 Stored Procedure 를 사용해서 데이터 업데이트 부분만 설명
            // EF Core기준으로 설명
            #region 한국 시간 얻어 오기 : DateTime의 Local Time 독립을 위해서
            int p = (int)Environment.OSVersion.Platform;
            bool isLinux = (p == 4) || (p == 6) || (p == 128);
            var checkTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, (isLinux) ? "Asia/Seoul" : "Korea Standard Time");
            #endregion

            var txId = Guid.NewGuid().ToString("N");

            try
            {

                #region EMONEY DB에 출금 트랜젝션를 추가 한다.
                var trans = new TbBankTran
                {
                    TrDate = checkTime.ToString("yyyyMMdd"),
                    /* Seq : DB에서 자동생성 */
                    /* TrSeq : 서비스에서 자동생성 */
                    // 가입자 은행 : 환경 설정에 있는 값을 가져다 쓴다.
                    OrgBank = _config["FirmBanking:OrgBank"],
                    // 기관 코드 : 환경 설정에 있는 값을 가져다 쓴다.
                    OrgCd = _config["FirmBanking:OrgCd"],
                    // 출금 은행 코드 : 환경 설정에 있는 값을 가져다 쓴다.
                    OutBankCd = _config["FirmBanking:OutBankCd"],
                    // 출금 계좌 번호 : 환경 설정에 있는 값을 가져다 쓴다.
                    OutAcctNo = _config["FirmBanking:OutAcctNo"],
                    // 입금할 은행 코드 : 현재는 숫자로 입력 받아서 BankCode.ToString("D3") 로 사용
                    // 입금 가능 은행 체크하기 위해서
                    InBankCd = model.BankCode,
                    // 입금할 계좌 번호
                    InAcctNo = model.AccountNo,
                    // 출금 금액
                    TrAmt = model.TransferAmount,
                    // 출금 트랜젝션 기록 : 현재는 "송금이체하기"로 넣어주고 있음
                    OutName = model.OutName,
                    // 입금자 이름
                    RemiName = model.Name,
                    // 트랜젝션 시간
                    EntryDate = checkTime.ToString("yyyyMMddHHmmss"),
                    // 사용 이용서비스 구분자 : Gloss의 경우 "BR_"를 Prefix로 사용하여 각 서비스의별 구분을 하고 있음 
                    // 실제로 출금시 이를 구분하여 각 서비스의 트랜젝션 상태를 체크함
                    EntryIdno = $"A21012001m"
                };

                #region 출금 요청 DB 테이블에 등록해 준다 : 동기화 처리를 사용한다.

                var result = await _transferService.InsertBankTran(trans);
                #endregion

                #endregion

                #region 출금 트랜젝션 테이블에 등록 : 저장후에 등록을 해야 SEQ 값이 나온다.
                var txTableId = $"{trans.TrDate}:{result}:{trans.OrgBank}:{trans.OrgCd}";
                _txTable.TryAdd(txTableId, $"{model.MID}:{txId}");
                await _walletService.InsertWalletLog(new WalletLog()
                {
                    Key = txTableId,
                    Value = $"{model.MID}:{txId}"
                });
                #endregion

                return CreatedAtRoute(
                        routeName: "GetTransactionAsync",
                        routeValues: new { mid = model.MID, txid = txId },
                        new
                        {
                            code = 0,
                            msg = "Success",
                            data = new { txId = txId }
                        });
            }
            catch (Exception ex)
            {
                //_dbContext.SaveChanges();
                //return Core.ViewModels.Responses.ResultDataOfT<ViewModels.Responses.CreateWithdrawAuth>
                //            .Create(request, -1, $"출금 처리에 장애가 발생했습니다.\n 잠시후 다시 이용해 주세요.");
                return Ok(
                    new
                    {
                        code = -1,
                        msg = $"출금 처리에 장애가 발생했습니다.\n 잠시후 다시 이용해 주세요.\n{ex.Message}"
                    }
                );
            }
        }

        /// <summary>
        /// 트랜젝션 상태 리턴
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("tx/{mid}/{id}", Name = "GetTransactionAsync")]
        [Produces("application/json")]
        public async Task<IActionResult>
                GetTransactionAsync(string mid, string id)
        {
            #region Dummy Code : 아래 코드에서 비동기 처리가 되기 때문에...
            await Task.Run(() => { });
            #endregion

            // 출금 트랜젝션 로그 상태값 리턴
            throw new NotImplementedException();
        }
        #endregion
    }
}

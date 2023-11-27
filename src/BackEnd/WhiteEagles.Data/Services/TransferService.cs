using System.Collections.Generic;
using WhiteEagles.Data.ViewModels;

namespace WhiteEagles.Data.Services
{
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MySql.Data.MySqlClient;

    using Models;

    public interface ITransferService
    {
        Task<long> InsertBankTran(TbBankTran info);
        Task<TbBankTran> SelectTranInfo(string trDate, int seq, string orgBank, 
            string orgCd);
        
    }
    public class TransferService : ITransferService
    {
        private readonly ILogger<TransferService> _logger;
        private readonly IConfiguration _configuration;

        public TransferService(ILogger<TransferService> logger, 
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<long> InsertBankTran(TbBankTran info)
        {

            var sqlText = @"
                    INSERT INTO TB_BANK_TRAN(TR_DATE, ORG_BANK, ORG_CD, OUT_BANK_CD, OUT_ACCT_NO, IN_BANK_CD, IN_ACCT_NO, TR_AMT, OUT_NAME, REMI_NAME, ENTRY_DATE,ENTRY_IDNO)
                                 VALUES(DATE_FORMAT(NOW(), '%Y%m%d'),@ORG_BANK,@ORG_CD,@OUT_BANK_CD,@OUT_ACCT_NO,@IN_BANK_CD,@IN_ACCT_NO,@TR_AMT, @OUT_NAME, @REMI_NAME, DATE_FORMAT(NOW(), '%Y%m%d%H%i%s'), @ENTRY_IDNO);
                    SELECT last_insert_id(); ";
            await using var connection = ConnectionFactory();
            return await connection.ExecuteScalarAsync<long>(sqlText,
                new
                {
                    ORG_BANK = info.OrgBank, ORG_CD = info.OrgCd,
                    OUT_BANK_CD = info.OutBankCd,
                    OUT_ACCT_NO = info.OutAcctNo,
                    IN_BANK_CD = info.InBankCd,
                    IN_ACCT_NO = info.InAcctNo,
                    TR_AMT = info.TrAmt,
                    OUT_NAME = info.OutName,
                    REMI_NAME = info.RemiName,
                    ENTRY_IDNO = info.EntryIdno
                });
        }

        public async Task<TbBankTran> SelectTranInfo(string trDate, int seq, 
            string orgBank, string orgCd)
        {
            var sqlText = @" SELECT TR_DATE As TrDate, SEQ AS Seq, ORG_BANK AS OrgBank, ORG_CD AS OrgCd, TR_SEQ AS TrSeq,  
OUT_BANK_CD AS OutBankCd, OUT_ACCT_NO AS OutAcctNo, IN_BANK_CD As InBankCd, IN_ACCT_NO As InAcctNo,
TR_AMT AS TrAmt, FEE AS Fee, BAL_SIGN AS BalSign, BAL_AMT As BalAmt, OUT_NAME As OutName, 
REMI_NAME As RemiName, CMS_CD AS CmsCd, PROC_FLAG AS ProcFlag, ERROR_CD AS ErrorCd, UNABLE AS Unable,
SEND_DATETIME As SendDatetime, RECV_DATETIME As RecvDatetime, ENTRY_DATE As EntryDate,  
ENTRY_IDNO AS EntryIdno, ERP_PROC_YN AS ErpProcYn, use_idx AS UseIdx
FROM TB_BANK_TRAN
WHERE TR_DATE = @TR_DATE AND SEQ = @SEQ AND ORG_BANK = @ORG_BANK AND ORG_CD = @ORG_CD
";
            await using var connection = ConnectionFactory();

            return await connection.QueryFirstOrDefaultAsync<TbBankTran>(sqlText, new
            {
                TR_DATE = trDate,
                SEQ = seq,
                ORG_BANK = orgBank,
                ORG_CD = orgCd
            });
        }

        private MySqlConnection ConnectionFactory()
        {
            var connectionString = _configuration["ConnectionStrings:Default"];
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}

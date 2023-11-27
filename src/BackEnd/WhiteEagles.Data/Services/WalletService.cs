namespace WhiteEagles.Data.Services
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;
    using System.Text;

    using Models;
    using ViewModels;

    public interface IWalletService
    {
        public Task<int> InsertWalletLog(WalletLog info);
        public Task<int> DeleteWalletLog(string key);
        public Task<IEnumerable<WalletLog>> SelectWalletLog();
        public Task<int> InsertWalletHistory(WalletHistory info);
        public Task<IEnumerable<WalletHistory>> SelectWalletHistory(string mId);
        public Task<IEnumerable<WalletSumHistory>> SelectWalletSumHistory(string mId);
        public Task<LimitRegisterViewModel> SelectAvailableLimit(string mId);
        public Task RegisterLimit(LimitRegisterInfo info);
        public Task RegisterTransferSuccess(TbBankTran info);
        Task<IEnumerable<WalletHistoryResultView>>
            SelectWalletHistory(WalletHistoryRequestView info);
        Task<IEnumerable<WalletHistoryResultView>>
            SelectWalletHistoryAll(WalletHistoryRequestView info);

        Task<IEnumerable<AvailableLimitInfo>> SelectAvailableLimitAll(string mid);

        Task<int> SelectWalletHistoryCount(WalletHistoryRequestView info);
    }

    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly IConfiguration _configuration;

        public WalletService(ILogger<WalletService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        public async Task<int> InsertWalletLog(WalletLog info)
        {
            var sqlText = @" INSERT INTO WalletLog(Key, Value, TimeStamp)
                            values(@Key, @Value, Now())";

            await using var connection = ConnectionFactory();

            return await connection.ExecuteAsync(sqlText, 
                new
                    {
                        Key = info.Key,
                        Value = info.Value

                    });

        }

        public async Task<int> DeleteWalletLog(string key)
        {
            var sqlText = @"delete from WalletLog 
                            where Key = @Key ";

            await using var connection = ConnectionFactory();

            return await connection.ExecuteAsync(sqlText,
                new
                {
                    Key = key

                });
        }

        public async Task<IEnumerable<WalletLog>> SelectWalletLog()
        {
            var sqlText = " select `Key`, Value from walletlog ";

            await using var connection = ConnectionFactory();

            return connection.Query<WalletLog>(sqlText);
        }

        

        public async Task<IEnumerable<WalletHistory>> SelectWalletHistory(string mId)
        {
            var sqlText = @"select id, MerchantId, Type, Amount, Balance, FeeType, FeeByCase, Fee, TimeStamp
            from walletHistory where MerchantId = @MerchantId Order by TimeStamp";

            await using var connection = ConnectionFactory();

            return await connection.QueryAsync<WalletHistory>(sqlText,
                new
                {
                    MerchantId = mId
                });
        }

        
        public async Task<IEnumerable<WalletSumHistory>> SelectWalletSumHistory(string mId)
        {
            var sqlText = @"SELECT MerchantId, Date, DepositSum, WithdrawalSum, TimeStamp
From walletsumhistory
where MerchantId = @MerchantId";

            await using var connection = ConnectionFactory();

            return await connection.QueryAsync<WalletSumHistory>(sqlText,
                new
                {
                    MerchantId = mId
                });
        }

        

        

        public async Task<LimitRegisterViewModel> SelectAvailableLimit(string mId)
        {
            var sqlText = @" Select a.MerchantId, a.SuspenseReceipts, IFNULL(b.WithdrawalSum, 0) As WithdrawalTotalAmount,
  IFNULL(c.DepositSum, 0) - IFNULL(c.WithdrawalSum, 0) AS PreviousBalanceAmount,
  IFNULL(b.DepositSum, 0) As DepositTotalAmount
  
from merchantinfo a left outer join 
(
select MerchantId, WithdrawalSum, DepositSum from walletsumhistory
where Date = Date_Format(Now(),'%Y%m') and MerchantId = @MerchantID1
) b on a.MerchantId = b.MerchantId
left outer join 
(
  select MerchantId, Sum(WithdrawalSum) As WithdrawalSum, sum(DepositSum) As DepositSum
  from walletsumhistory
  where Date < Date_Format(Now(),'%Y%m') and MerchantId = @MerchantID2
) c on a.MerchantId = c.MerchantId ";

            await using var connection = ConnectionFactory();

            return await connection.QueryFirstOrDefaultAsync<LimitRegisterViewModel>(sqlText,
                new
                {
                    MerchantId1 = mId,
                    MerchantId2 = mId
                });

        }

        public async Task<IEnumerable<AvailableLimitInfo>> SelectAvailableLimitAll(string mid)
        {
            var sqlText = new StringBuilder();
            sqlText.Append(@" select a.MerchantId, a.SuspenseReceipts, IFNULL(b.DepositSum, 0) As DepositSum ,  
IFNULL(b.WithdrawalSum, 0) As WithdrawalSum
from 
merchantinfo a left outer join 
( select MerchantId, sum(DepositSum) As DepositSum, sum(WithdrawalSum) As WithdrawalSum
from walletsumhistory
group by MerchantId
) b on (a.MerchantId = b.MerchantId)");
                

            if (!String.IsNullOrEmpty(mid))
            {
                sqlText.Append($" where a.MerchantId Like '%{mid}%' ");

            }

            await using var connection = ConnectionFactory();

            return await connection.QueryAsync<AvailableLimitInfo>(sqlText.ToString());

        }


        public async Task RegisterLimit(LimitRegisterInfo info)
        {
            await using var connection = ConnectionFactory();

            await using var tran = connection.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var limitInfo = await SelectAvailableLimit(info.MerchantId, tran);
                var balance = limitInfo.SuspenseReceipts + limitInfo.DepositSum - limitInfo.WithdrawalSum + info.RegisterAmount;
                var walletHistory = new WalletHistory()
                {
                    MerchantId = info.MerchantId,
                    Type = "Deposit",
                    Amount = info.RegisterAmount,
                    Balance = balance
                };
                await InsertWalletHistory(walletHistory, tran);

                var count = await SelectWalletSumHistoryCount(info.MerchantId, tran);

                var walletSumHistory = new WalletSumHistory()
                {
                    DepositSum = info.RegisterAmount,
                    WithdrawalSum = 0,
                    MerchantId = info.MerchantId
                };


                if (count > 0)
                {
                    await UpdateWalletSumHistory(walletSumHistory, tran);
                }
                else
                {
                    await InsertWalletSumHistory(walletSumHistory, tran);
                }


                await tran.CommitAsync();

            }
            catch (Exception e)
            {
                _logger.LogInformation($"RegisterLimit Method Exception {e.Message}");
                await tran.RollbackAsync();
                throw;
            }


        }

        public async Task RegisterTransferSuccess(TbBankTran info)
        {
            await using var connection = ConnectionFactory();

            await using var tran = connection.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var limitInfo = await SelectAvailableLimit(info.EntryIdno, tran);
                var balance = limitInfo.SuspenseReceipts + limitInfo.DepositSum - limitInfo.WithdrawalSum - info.TrAmt;
                var walletHistory = new WalletHistory()
                {
                    MerchantId = info.EntryIdno,
                    Type = "Withdrawal",
                    Amount = info.TrAmt,
                    Balance = balance
                };
                await InsertWalletHistory(walletHistory, tran);

                var count = await SelectWalletSumHistoryCount(info.EntryIdno, tran);

                var walletSumHistory = new WalletSumHistory()
                {
                    WithdrawalSum = info.TrAmt,
                    DepositSum = 0,
                    MerchantId = info.EntryIdno
                };


                if (count > 0)
                {
                    await UpdateWalletSumHistory(walletSumHistory, tran);
                }
                else
                {
                    await InsertWalletSumHistory(walletSumHistory, tran);
                }


                await tran.CommitAsync();

            }
            catch (Exception e)
            {
                _logger.LogInformation($"RegisterTransferSuccess Method Exception {e.Message}");
                await tran.RollbackAsync();
                throw;
            }

        }

        public async Task<IEnumerable<WalletHistoryResultView>> SelectWalletHistory(WalletHistoryRequestView info)
        {
            var sqlText = new StringBuilder();
            sqlText.Append(@" select a.Entry_Idno As MerchantName, a.Entry_Idno As MerchantId, 
                b.BankName As BankName, a.REMI_NAME As AccountName, a.IN_ACCT_NO As AccountNo, 
                a.SEND_DATETIME As SendDateTime, a.TR_AMT As Amount, a.ERROR_CD As Result, c.ResponseName As ResultMessage
                from TB_BANK_TRAN a inner join bankinfo b on(a.ORG_BANK = b.bankCode)
            inner join responsecodeinfo c on(a.ERROR_CD = c.ResponseCode)
            where tr_date between @StartDate and @EndDate ");


            if (!String.IsNullOrEmpty(info.Mid))
            {
                sqlText.Append($" and a.Entry_Idno = '{info.Mid}' ");
            }

            if (!String.IsNullOrEmpty(info.BankCode))
            {
                sqlText.Append($" and a.OUT_BANK_CD = '{info.BankCode}' ");
            }

            if (!String.IsNullOrEmpty(info.InAccountNo))
            {
                sqlText.Append($" and a.IN_ACCT_NO Like '%{info.InAccountNo}%' ");
            }

            if (!String.IsNullOrEmpty(info.RemiName))
            {
                sqlText.Append($" and a.Remi_Name Like '%{info.RemiName}%' ");
            }

            if (!String.IsNullOrEmpty(info.TransferStatus))
            {
                if (info.TransferStatus == "Y")
                {
                    sqlText.Append($" and a.Error_Cd = '000' ");
                }
                else
                {
                    sqlText.Append($" and a.Error_Cd != '000' ");
                }

            }

            sqlText.Append($" order by a.SEND_DATETIME desc limit {info.Page}, {info.PageCount} ");

            await using var connection = ConnectionFactory();

            return await connection.QueryAsync<WalletHistoryResultView>(
                sqlText.ToString(),
                new
                {
                    StartDate = info.StartDate,
                    EndDate = info.EndDate
                });

        }

        public async Task<IEnumerable<WalletHistoryResultView>> SelectWalletHistoryAll(WalletHistoryRequestView info)
        {
            var sqlText = new StringBuilder();
            sqlText.Append(@" select a.Entry_Idno As MerchantName, a.Entry_Idno As MerchantId, 
                b.BankName As BankName, a.REMI_NAME As AccountName, a.IN_ACCT_NO As AccountNo, 
                a.SEND_DATETIME As SendDateTime, a.TR_AMT As Amount, a.ERROR_CD As Result, c.ResponseName As ResultMessage
                from TB_BANK_TRAN a inner join bankinfo b on(a.ORG_BANK = b.bankCode)
            inner join responsecodeinfo c on(a.ERROR_CD = c.ResponseCode)
            where tr_date between @StartDate and @EndDate ");


            if (!String.IsNullOrEmpty(info.Mid))
            {
                sqlText.Append($" and a.Entry_Idno = '{info.Mid}' ");
            }

            if (!String.IsNullOrEmpty(info.BankCode))
            {
                sqlText.Append($" and a.OUT_BANK_CD = '{info.BankCode}' ");
            }

            if (!String.IsNullOrEmpty(info.InAccountNo))
            {
                sqlText.Append($" and a.IN_ACCT_NO Like '%{info.InAccountNo}%' ");
            }

            if (!String.IsNullOrEmpty(info.RemiName))
            {
                sqlText.Append($" and a.Remi_Name Like '%{info.RemiName}%' ");
            }

            if (!String.IsNullOrEmpty(info.TransferStatus))
            {
                if (info.TransferStatus == "Y")
                {
                    sqlText.Append($" and a.Error_Cd = '000' ");
                }
                else
                {
                    sqlText.Append($" and a.Error_Cd != '000' ");
                }

            }

            await using var connection = ConnectionFactory();

            return await connection.QueryAsync<WalletHistoryResultView>(
                sqlText.ToString(),
                new
                {
                    StartDate = info.StartDate,
                    EndDate = info.EndDate
                });

        }

        public async Task<int> SelectWalletHistoryCount(WalletHistoryRequestView info)
        {
            var sqlText = new StringBuilder();
            sqlText.Append(@" select Count(*)
from TB_BANK_TRAN a inner join bankinfo b on (a.ORG_BANK = b.bankCode)
inner join responsecodeinfo c on (a.ERROR_CD = c.ResponseCode)
where tr_date between @StartDate and @EndDate ");

            if (!String.IsNullOrEmpty(info.Mid))
            {
                sqlText.Append($" and a.Entry_Idno = '{info.Mid}' ");
            }

            if (!String.IsNullOrEmpty(info.BankCode))
            {
                sqlText.Append($" and a.OUT_BANK_CD = '{info.BankCode}' ");
            }

            if (!String.IsNullOrEmpty(info.InAccountNo))
            {
                sqlText.Append($" and a.IN_ACCT_NO Like '%{info.InAccountNo}%' ");
            }

            if (!String.IsNullOrEmpty(info.RemiName))
            {
                sqlText.Append($" and a.Remi_Name Like '%{info.RemiName}%' ");
            }

            if (!String.IsNullOrEmpty(info.TransferStatus))
            {
                if (info.TransferStatus == "Y")
                {
                    sqlText.Append($" and a.Error_Cd = '000' ");
                }
                else
                {
                    sqlText.Append($" and a.Error_Cd != '000' ");
                }

            }

            await using var connection = ConnectionFactory();

            return await connection.ExecuteScalarAsync<int>(sqlText.ToString(),
                new
                {
                    StartDate = info.StartDate,
                    EndDate = info.EndDate
                });
        }


        private async Task<AvailableLimitInfo> SelectAvailableLimit(string mId, 
            MySqlTransaction tran)
        {
            var sqlText = @" select b.MerchantId, IFNULL(a.DepositSum, 0) AS DepositSum, IFNULL(a.WithdrawalSum, 0) As WithdrawalSum, 
b.SuspenseReceipts
from
(
select MerchantId, sum(DepositSum) As DepositSum, sum(WithdrawalSum) As WithdrawalSum
from walletsumhistory
group by MerchantId
) a right outer join merchantinfo b on (a.MerchantId = b.MerchantId)
where B.MerchantID = @MerchantId ";

            await using var connection = ConnectionFactory();

            return await connection.QueryFirstOrDefaultAsync<AvailableLimitInfo>(sqlText,
                new
                {
                    MerchantId = mId
                }, tran);

        }

        public async Task<int> InsertWalletHistory(WalletHistory info)
        {
            var sqlText =
                @"INSERT INTO walletHistory(MerchantId, Type, Amount, Balance, FeeType, FeeByCase, Fee, TimeStamp)
                        Values(@MerchantId, @Type, @Amount, @Balance, @FeeType, @FeeByCase, @Fee, Now())";


            await using var connection = ConnectionFactory();

            return await connection.ExecuteAsync(sqlText,
                new
                {
                    MerchantId = info.MerchantId,
                    Type = info.Type,
                    Amount = info.Amount,
                    Balance = info.Balance,
                    FeeType = info.FeeType,
                    FeeByCase = info.FeeByCase,
                    Fee = info.Fee
                });

        }


        private async Task<int> InsertWalletHistory(WalletHistory info, MySqlTransaction tran)
        {
            var sqlText =
                @"INSERT INTO walletHistory(MerchantId, Type, Amount, Balance, FeeType, FeeByCase, Fee, TimeStamp)
                        Values(@MerchantId, @Type, @Amount, @Balance, @FeeType, @FeeByCase, @Fee, Now())";


            await using var connection = ConnectionFactory();

            return await connection.ExecuteAsync(sqlText,
                new
                {
                    MerchantId = info.MerchantId,
                    Type = info.Type,
                    Amount = info.Amount,
                    Balance = info.Balance,
                    FeeType = info.FeeType,
                    FeeByCase = info.FeeByCase,
                    Fee = info.Fee
                }, tran);

        }

        private async Task<int> SelectWalletSumHistoryCount(string mId, MySqlTransaction tran)
        {
            var sqlText = @" SELECT Count(*)
From walletsumhistory
where MerchantId = @MerchantId and Date = Date_Format(Now(),'%Y%m') ";

            await using var connection = ConnectionFactory();

            return await connection.ExecuteScalarAsync<int>(sqlText,
                new
                {
                    MerchantId = mId
                }, tran);
        }

        private async Task<int> InsertWalletSumHistory(WalletSumHistory info, MySqlTransaction tran)
        {
            var sqlText = @" INSERT INTO walletsumhistory(MerchantId, Date, DepositSum, WithdrawalSum, TimeStamp)
Values(@MerchantId, Date_Format(Now(),'%Y%m'), @DepositSum, @WithdrawalSum, Now())";


            await using var connection = ConnectionFactory();

            return await connection.ExecuteAsync(sqlText,
                new
                {
                    MerchantId = info.MerchantId,
                    DepositSum = info.DepositSum,
                    WithdrawalSum = info.WithdrawalSum
                }, tran);

        }


        private async Task<int> UpdateWalletSumHistory(WalletSumHistory info, MySqlTransaction tran)
        {
            var sqlText = @"Update walletsumhistory
                set
                DepositSum = DepositSum + @DepositSum,
                WithdrawalSum = WithdrawalSum + @WithdrawalSum,
                DepositCount = DepositCount + 1,
                WithdrawalCount = WithdrawalCount + 1,
                TimeStamp = Now()
                where MerchantId = @MerchantId and Date = Date_Format(Now(),'%Y%m')
                ";

            await using var connection = ConnectionFactory();

            return await connection.ExecuteAsync(sqlText,
                new
                {
                    DepositSum = info.DepositSum,
                    WithdrawalSum = info.WithdrawalSum,
                    MerchantId = info.MerchantId
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
 
namespace WhiteEagles.Data.Services
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Dapper;
    using MySql.Data.MySqlClient;

    using Models;

    public interface IMerchantInfoService
    {
        public Task<int> InsertMerchantInfo(MerchantInfo info);
        public Task<int> UpdateMerchantInfo(MerchantInfo info);
        public Task<MerchantInfo> SelectMerchantInfo(string merchantId);
        public Task<string> SelectMerchantCode();
        public Task<int> SelectMerchantCount();

    }

    public class MerchantInfoService : IMerchantInfoService
    {
        private readonly ILogger<MerchantInfoService> _logger;
        private readonly IConfiguration _configuration;

        public MerchantInfoService(ILogger<MerchantInfoService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<int> InsertMerchantInfo(MerchantInfo info)
        {
            var sqlText = @"INSERT INTO merchantinfo(MerchantId, MerchantCode, ServiceUse, ServiceStartDate, SuspenseReceipts, FeeType, MinFee, MinLimit, FeeByCase,  MonthBaseFee, CalculateDay)
                        Values(@MerchantId, @MerchantCode, @ServiceUse, @ServiceStartDate, @SuspenseReceipts, @FeeType, @MinFee, @MinLimit, @FeeByCase, @MonthBaseFee, @CalculateDay)";

            await using var connection = ConnectionFactory();
            return await connection.ExecuteAsync(sqlText,
                new
                {
                    MerchantId = info.MerchantId,
                    MerchantCode = info.MerchantCode,
                    ServiceUse = info.ServiceUse,
                    ServiceStartDate = info.ServiceStartDate,
                    SuspenseReceipts = info.SuspenseReceipts,
                    FeeType = info.FeeType,
                    MinFee = info.MinFee,
                    MinLimit = info.MinLimit,
                    FeeByCase = info.FeeByCase,
                    MonthBaseFee = info.MonthBaseFee,
                    CalculateDay = info.CalculateDay
                });
        }

        public async Task<int> UpdateMerchantInfo(MerchantInfo info)
        {
            var sqlText = @"UPDATE merchantinfo
                    SET
                    ServiceUse = @ServiceUse,
                    ServiceStartDate = @ServiceStartDate,
                    SuspenseReceipts = @SuspenseReceipts,
                    FeeType = @FeeType,
                    MinFee = @MinFee,
                    MinLimit = @MinLimit,
                    FeeByCase = @FeeByCase,
                    MonthBaseFee = @MonthBaseFee,
                    CalculateDay = @CalculateDay
                    where MerchantId = @MerchantId";

            await using var connection = ConnectionFactory();
            return await connection.ExecuteAsync(sqlText,
                new
                {
                    ServiceUse = info.ServiceUse,
                    ServiceStartDate = info.ServiceStartDate,
                    SuspenseReceipts = info.SuspenseReceipts,
                    FeeType = info.FeeType,
                    MinFee = info.MinFee,
                    MinLimit = info.MinLimit,
                    FeeByCase = info.FeeByCase,
                    MonthBaseFee = info.MonthBaseFee,
                    CalculateDay = info.CalculateDay,
                    MerchantId = info.MerchantId
                });


        }

        public async Task<MerchantInfo> SelectMerchantInfo(string merchantId)
        {
            var sqlText = @"SELECT MerchantId, MerchantCode, ServiceUse, date_format(ServiceStartDate, '%Y-%m-%d 00:00:00') as ServiceStartDate, SuspenseReceipts, FeeType, MinFee, MinLimit, FeeByCase,  MonthBaseFee, CalculateDay
                FROM merchantinfo
                WHERE MerchantId = @MerchantId";

            await using var connection = ConnectionFactory();

            return await connection.QuerySingleOrDefaultAsync<MerchantInfo>(sqlText,
                new
                {
                    MerchantId = merchantId
                });
        }

        public async Task<string> SelectMerchantCode()
        {
            var sqlText = @"select IFNULL(MerchantCode, 'TR_0001') from merchantinfo
                order by TimeStamp Desc
                limit 1";

            await using var connection = ConnectionFactory();

            return await connection.QuerySingleOrDefaultAsync<string>(sqlText);
        }

        public async Task<int> SelectMerchantCount()
        {
            var sqlText = @" select Count(*) As Count from merchantinfo ";

            await using var connection = ConnectionFactory();

            return await connection.ExecuteScalarAsync<int>(sqlText);
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

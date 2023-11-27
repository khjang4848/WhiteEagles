namespace WhiteEagles.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MySql.Data.MySqlClient;

    using ViewModels;

    public interface ICalculateService
    {
        public Task<IEnumerable<CalculateViewModel>> SelectCalculateInfo(
            string startDate, string endDate, string mid);
        public Task<IEnumerable<CalculateViewModel>> SelectCalculateInfoAll(
            string startDate, string endDate, string mid);
    }
    public class CalculateService : ICalculateService
    {
        private readonly ILogger<CalculateService> _logger;
        private readonly IConfiguration _configuration;

        public CalculateService(ILogger<CalculateService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<CalculateViewModel>> SelectCalculateInfo(
            string startDate, string endDate, string mid)
        {
            var sqlText = new StringBuilder();
            sqlText.Append(@" select a.MerchantId, a.Date, a.WithdrawalCount, a.WithdrawalSum As WithdrawalAmount, 
b.FeeType, b.MinFee, b.MinLimit, b.MonthBaseFee, b.CalculateDay, 0 As BillingAmount, 0 As SurtaxAmount, 0 As TotalAmount
from walletsumhistory a inner join merchantinfo b on (a.MerchantId = b.MerchantId)
where a.Date Between @StartDate and @EndDate ");

            if (!String.IsNullOrEmpty(mid))
            {
                sqlText.Append($" and a.MerchantId = '{mid}' ");
            }

            await using var connection = ConnectionFactory();

            return await connection.QueryAsync<CalculateViewModel>(sqlText.ToString(),
                new
                {
                    StartDate = startDate,
                    EndDate = endDate
                });

        }

        public async Task<IEnumerable<CalculateViewModel>> SelectCalculateInfoAll(
            string startDate, string endDate, string mid)
        {
            var sqlText = new StringBuilder();
            sqlText.Append(@" select a.MerchantId, a.Date, a.WithdrawalCount, a.WithdrawalSum As WithdrawalAmount, 
b.FeeType, b.MinFee, b.MinLimit, b.MonthBaseFee, b.CalculateDay, 0 As BillingAmount, 0 As SurtaxAmount, 0 As TotalAmount
from walletsumhistory a inner join merchantinfo b on (a.MerchantId = b.MerchantId)
where a.Date Between @StartDate and @EndDate ");

            if (!String.IsNullOrEmpty(mid))
            {
                sqlText.Append($" and a.MerchantId in ({mid}) ");
            }

            await using var connection = ConnectionFactory();

            return await connection.QueryAsync<CalculateViewModel>(sqlText.ToString(),
                new
                {
                    StartDate = startDate,
                    EndDate = endDate
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

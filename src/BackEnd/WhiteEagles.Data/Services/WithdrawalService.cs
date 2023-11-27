namespace WhiteEagles.Data.Services
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MySql.Data.MySqlClient;

    public interface IWithdrawalService
    {   
    }

    public class WithdrawalService : IWithdrawalService
    {
        private readonly ILogger<WithdrawalService> _logger;
        private readonly IConfiguration _configuration;

        public WithdrawalService(ILogger<WithdrawalService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

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

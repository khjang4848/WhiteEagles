namespace WhiteEagles.Data.Services
{
    using System;
    using System.Data;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MySql.Data.MySqlClient;

    using DomainModels;
    using ViewModels;

    using WhiteEagles.Infrastructure.Serialization;

    public interface IAccountInquiryService
    {
        Task<AccountInquiryResponseViewModel> AccountInquiryAsync(
            AccountInquiryViewModel request);
    }


    public class AccountInquiryService : IAccountInquiryService
    {
        private readonly ITextSerializer _textSerializer;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountInquiryService> _logger;

        public AccountInquiryService(ITextSerializer textSerializer,
            IConfiguration config, ILogger<AccountInquiryService> logger)
        {
            _textSerializer = textSerializer ??
                    throw new ArgumentNullException(nameof(textSerializer));
            _config = config;
            _logger = logger;
        }

        public async Task<AccountInquiryResponseViewModel> AccountInquiryAsync(
            AccountInquiryViewModel request)
        {
            var (id, seq) = SelectSeqAndId(request);

            var sequenceNo = $"0{seq.ToString().PadRight(5, '0')}9";
            var requestInfo = new AccountInquiryRequest()
            {
                SecurityKey = _config["AccountInquiry:SecurityKey"],
                Key = _config["AccountInquiry:Key"],
                RequestData = new[]
                {
                    new AccountInquiryRequestData()
                    {
                        AccountNo = request.AccountNo,
                        BankCode = request.BankCode,
                        SearchAccountNo = request.SearchAccountNo,
                        TransactionSeqNo = sequenceNo,
                        TransferAmount = request.TransferAmount.ToString()
                    }
                }

            };

            var serializeText = _textSerializer.Serialize(requestInfo);
            var jsonData = HttpUtility.UrlEncode(serializeText);

            _logger.LogWarning($"요청 Parameter = {serializeText}");

            jsonData = $"{_config["AccountInquiry:Url"]}?JSONData={jsonData}";

            var httpClient = new HttpClient();

            var result = await httpClient.PostAsync(
                jsonData, null);

            result.EnsureSuccessStatusCode();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var resultString = await result.Content.ReadAsByteArrayAsync();
            var resultStringEncoding = Encoding.GetEncoding(51949).GetString(resultString);

            var resultModel = _textSerializer.Deserialize<AccountInquiryResponseViewModel>(
                resultStringEncoding);

            _logger.LogWarning($"결과 Parameter = {_textSerializer.Serialize(resultModel)}");

            UpdateAccountInquiryResult(resultModel.ResultCode, resultModel.ResultMessage, id);

            return resultModel;
        }

        private (int, int) SelectSeqAndId(AccountInquiryViewModel request)
        {
            using var connection = new MySqlConnection(GetConnectionString());
            connection.Open();

            var seq1 = 0;
            var Id = 0;

            var command = new MySqlCommand("usp_account_inquiry", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("?BankCode", MySqlDbType.String)
                .Value = request.BankCode;
            command.Parameters.Add("?AcctNo", MySqlDbType.String)
                .Value = request.SearchAccountNo;
            command.Parameters.Add("?AcnmNo", MySqlDbType.String)
                .Value = request.AccountNo;
            command.Parameters.Add("?IcheAmt", MySqlDbType.Int32)
                .Value = request.TransferAmount;
            command.Parameters.Add("?Seq1", MySqlDbType.Int32)
                .Direction = ParameterDirection.Output;
            command.Parameters.Add("?Id", MySqlDbType.Int32)
                .Direction = ParameterDirection.Output;

            command.ExecuteNonQuery();


            if (command.Parameters["?Seq1"] != null)
            {
                seq1 = (int)command.Parameters["?Seq1"].Value;
            }

            if (command.Parameters["?Seq1"] != null)
            {
                Id = (int)command.Parameters["?Id"].Value;
            }

            connection.Close();

            return (Id, seq1);
        }

        private void UpdateAccountInquiryResult(string resultCode, string resultMessage, int id)
        {
            var queryText = new StringBuilder();

            queryText.Append("UPDATE TB_ACCOUNT_INQUIRY ");
            queryText.Append("SET ");
            queryText.Append("RSLT_CD = ?RSLT_CD, ");
            queryText.Append("RSLT_MG = ?RSLT_MG ");
            queryText.Append("WHERE ID = ?ID ");

            using var connection = new MySqlConnection(GetConnectionString());
            connection.Open();

            var command = new MySqlCommand(queryText.ToString(), connection);

            command.Parameters.Add("?RSLT_CD", MySqlDbType.String)
                .Value = resultCode;
            command.Parameters.Add("?RSLT_MG", MySqlDbType.String)
                .Value = resultMessage;
            command.Parameters.Add("?ID", MySqlDbType.Int32).Value = id;

            command.ExecuteNonQuery();
            connection.Close();

        }

        private string GetConnectionString()
            => _config["ConnectionStrings:Default"];
    }

}

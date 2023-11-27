namespace WhiteEagles.Data.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    
    using WhiteEagles.Infrastructure.Serialization;
    using ViewModels.PGInquiry;

    public interface IPGInquiryService
    {
        Task<PGInquiryMerchantInfo> PGInquiryMerchantInfoAsync(string mid);
        Task<long> PGInquirySalesSumInfo(string mid, string date);

    }
    
    public class PGInquiryService : IPGInquiryService
    {
        private readonly ITextSerializer _textSerializer;
        private readonly IConfiguration _config;
        private readonly ILogger<PGInquiryService> _logger;

        public PGInquiryService(ITextSerializer textSerializer,
            IConfiguration config, ILogger<PGInquiryService> logger)
        {
            _textSerializer = textSerializer ??
                              throw new ArgumentNullException(nameof(textSerializer));
            _config = config;
            _logger = logger;
        }


        public async Task<PGInquiryMerchantInfo> PGInquiryMerchantInfoAsync(string mid)
        {
            var requestUri = $"{_config["PGInquiry:MerchantInfoURI"]}?mid={mid}";

            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync(requestUri);

            result.EnsureSuccessStatusCode();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var resultStringEncoding = await result.Content.ReadAsStringAsync();

            var resultBaseModel = _textSerializer
                .Deserialize<PGInquiryBaseModel>(resultStringEncoding);

            if (resultBaseModel.resCode != 0)
            {
                return null;
            }

            var resultModel = _textSerializer
                .Deserialize<PGInquiryMerchantInfo>(resultStringEncoding);

            return resultModel;

        }

        public async Task<long> PGInquirySalesSumInfo(string mid, string date)
        {
            var requestUri = $"{_config["PGInquiry:SalesSumURI"]}?mid={mid}&date={date}";


            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync(requestUri);

            result.EnsureSuccessStatusCode();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var resultStringEncoding = await result.Content.ReadAsStringAsync();

            var resultBaseModel = _textSerializer
                .Deserialize<PGInquiryBaseModel>(resultStringEncoding);

            if (resultBaseModel.resCode != 0)
            {
                return 0;
            }

            var resultModel = _textSerializer
                .Deserialize<PGInquirySalesSumInfo>(resultStringEncoding);

            return resultModel.salesSum;
        }
    }
}

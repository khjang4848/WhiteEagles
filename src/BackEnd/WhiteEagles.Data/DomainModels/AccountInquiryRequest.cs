namespace WhiteEagles.Data.DomainModels
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class AccountInquiryRequest
    {
        [Required]
        [MaxLength(20)]
        [JsonProperty("SECR_KEY")]
        public string SecurityKey { get; set; }

        [Required]
        [MaxLength(16)]
        [JsonProperty("KEY")]
        public string Key { get; set; }


        [Required]
        [JsonProperty("REQ_DATA")]
        public AccountInquiryRequestData[] RequestData { get; set; }
    }
}

namespace WhiteEagles.Data.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class AccountInquiryResponseViewModel : ResponseBaseViewModel
    {
        [Required]
        [JsonProperty("RESP_DATA")]
        public AccountNameInfo[] AccountName { get; set; }
    }
}

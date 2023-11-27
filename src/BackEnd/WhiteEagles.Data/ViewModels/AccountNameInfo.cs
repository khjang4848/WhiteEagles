namespace WhiteEagles.Data.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class AccountNameInfo
    {
        [Required]
        [MaxLength(20)]
        [JsonProperty("ACCT_NM")]
        public string AccountName { get; set; }
        [Required]
        [StringLength(7)]
        [JsonProperty("TRSC_SEQ_NO")]
        public string TransactionSeqNo { get; set; }
    }
}

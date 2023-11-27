namespace WhiteEagles.Data.DomainModels
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class AccountInquiryRequestData
    {
        [Required]
        [StringLength(3)]
        [JsonProperty("BANK_CD")]
        public string BankCode { get; set; }

        [Required]
        [MaxLength(20)]
        [JsonProperty("SEARCH_ACCT_NO")]
        public string SearchAccountNo { get; set; }

        [MaxLength(10)]
        [JsonProperty("ACNM_NO")]
        public string AccountNo { get; set; }

        [MaxLength(15)]
        [JsonProperty("ICHE_AMT")]
        public string TransferAmount { get; set; }

        [Required]
        [StringLength(7)]
        [JsonProperty("TRSC_SEQ_NO")]
        public string TransactionSeqNo { get; set; }
    }
}

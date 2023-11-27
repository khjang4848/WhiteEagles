namespace WhiteEagles.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using Newtonsoft.Json;

    public class MerchantInfo
    {
        [Required]
        [JsonProperty("MerchantId")]
        public string MerchantId { get; set; }
        public string MerchantCode { get; set; }
        [Required]
        [DefaultValue("N")]
        [JsonProperty("ServiceUse")]
        public string ServiceUse { get; set; }
        public string ServiceStartDate { get; set; }
        [Required]
        [DefaultValue(0)]
        public int SuspenseReceipts { get; set; }
        [Required]
        [DefaultValue("1")]
        public string FeeType { get; set; }
        [Required]
        [DefaultValue(0)]
        public int MinFee { get; set; }
        [Required]
        [DefaultValue(0)]
        public int MinLimit { get; set; }
        [Required]
        [DefaultValue(0)]
        public int FeeByCase { get; set; }
        [Required]
        [DefaultValue(0)]
        public int MonthBaseFee { get; set; }
        [Required]
        [DefaultValue(10)]
        public int CalculateDay { get; set; }
    }
}

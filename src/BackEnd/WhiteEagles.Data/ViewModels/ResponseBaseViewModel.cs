namespace WhiteEagles.Data.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class ResponseBaseViewModel
    {
        [Required]
        [MaxLength(20)]
        [JsonProperty("RSLT_CD")]
        public string ResultCode { get; set; }

        [Required]
        [JsonProperty("RSLT_MSG")]
        public string ResultMessage { get; set; }
    }
}

namespace WhiteEagles.Data.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class LimitRegisterInfo
    {
        [Required]
        public string MerchantId { get; set; }
        [Required]
        public int RegisterAmount { get; set; }
    }
}

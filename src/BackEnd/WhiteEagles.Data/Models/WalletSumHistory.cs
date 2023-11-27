namespace WhiteEagles.Data.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    

    public class WalletSumHistory
    {
        [Required]
        public string MerchantId { get; set; }
        [DefaultValue(0)]
        public int DepositSum { get; set; }
        [DefaultValue(0)]
        public int WithdrawalSum { get; set; }
        
    }
}

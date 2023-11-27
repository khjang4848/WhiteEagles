namespace WhiteEagles.Data.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class WithdrawalViewModel
    {
        [Required]
        [StringLength(3)]
        public string BankCode { get; set; }

        [Required]
        [MaxLength(20)]
        public string AccountNo { get; set; }

        [Required]
        [DefaultValue(0)]
        public int TransferAmount { get; set; }


        [Required]
        [MaxLength(16)]
        public string Name { get; set; }

        [Required]
        [MaxLength(16)]
        public string OutName { get; set; }

        // ReSharper disable once InconsistentNaming
        [MaxLength(100)]
        [DefaultValue("")]
        public string MID { get; set; }
    }
}

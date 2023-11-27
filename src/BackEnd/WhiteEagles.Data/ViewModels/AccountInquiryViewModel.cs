namespace WhiteEagles.Data.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class AccountInquiryViewModel
    {
        [Required]
        [StringLength(3)]
        public string BankCode { get; set; }

        [Required]
        [MaxLength(20)]
        public string SearchAccountNo { get; set; }

        [MaxLength(10)]
        public string AccountNo { get; set; }

        [DefaultValue(0)]
        public int TransferAmount { get; set; }
    }
}

namespace WhiteEagles.Data.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class WalletHistoryRequestView
    {
        public string Mid { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
        public string RemiName { get; set; }
        public string InAccountNo { get; set; }
        public string BankCode { get; set; }
        public string TransferStatus { get; set; }
        public int Page { get; set; } = 0;
        public int PageCount { get; set; } = 10;
    }
}

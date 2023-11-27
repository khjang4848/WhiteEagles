namespace WhiteEagles.Data.ViewModels
{
    public class LimitRegisterViewModel
    {
        public string MerchantName { get; set; }
        public string MerchantNo { get; set; }
        public string MerchantId { get; set; }
        public int SuspenseReceipts { get; set; }
        public int TotalSalesAmount { get; set; }
        public int WithdrawalTotalAmount { get; set; }
        public int DepositTotalAmount { get; set; }
        public int PreviousBalanceAmount { get; set; }

    }
}

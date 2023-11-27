namespace WhiteEagles.Data.ViewModels
{
    public class WalletHistoryResultView
    {
        public string MerchantName { get; set; }
        public string MerchantId { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string SendDateTime { get; set; }
        public int Amount { get; set; }
        public string Result { get; set; }
        public string ResultMessage { get; set; }
    }
}

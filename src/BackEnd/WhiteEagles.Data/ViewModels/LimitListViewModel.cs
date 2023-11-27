namespace WhiteEagles.Data.ViewModels
{
    public class LimitListViewModel
    {
        public string MerchantName { get; set; }
        public string MerchantNo { get; set; }
        public string MerchantId { get; set; }
        public string TelNo { get; set; }
        public long SuspenseReceipts { get; set; }
        public long PreSalesAmount { get; set; }
        public int TransferLimit { get; set; }
        public int AvailableLimit { get; set; }
    }
}

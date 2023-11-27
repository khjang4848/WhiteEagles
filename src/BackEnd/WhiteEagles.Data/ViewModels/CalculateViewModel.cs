namespace WhiteEagles.Data.ViewModels
{
    public class CalculateViewModel
    {
        public string MerchantName { get; set; }
        public string MerchantNo { get; set; }
        public string MerchantId { get; set; }
        public string TelNo { get; set; }
        public string Date { get; set; }
        public int WithdrawalCount { get; set; }
        public int WithdrawalAmount { get; set; }
        public string FeeType { get; set;}
        public int MinFee { get; set; }
        public int MinLimit { get; set; }
        public int FeeByCase { get; set;}
        public int MonthBaseFee { get; set; }
        public int CalculateDay { get; set; }
        public int BillingAmount { get; set; } = 0;
        public int SurtaxAmount { get; set; } = 0;
        public int TotalAmount{ get; set; } = 0;

    }
}

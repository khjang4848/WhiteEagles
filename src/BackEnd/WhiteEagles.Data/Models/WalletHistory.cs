namespace WhiteEagles.Data.Models
{
    using System;

    public class WalletHistory
    {
        public long Id { get; set; }
        public string MerchantId { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public string FeeType { get; set; }
        public int FeeByCase { get; set; }
        public int Fee { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}

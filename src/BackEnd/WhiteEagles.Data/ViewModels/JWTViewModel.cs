namespace WhiteEagles.Data.ViewModels
{
    // ReSharper disable once InconsistentNaming
    public class JWTViewModel
    {
        public string Code { get; set; }
        public string UserRole { get; set; }
        public string Token { get; set; }
        public long Expired { get; set; }
        public string ResCode { get; set; }
        public string ResMessage { get; set; }
    }
}

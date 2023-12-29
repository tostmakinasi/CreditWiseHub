namespace CreditWiseHub.Core.Configurations
{
    public class CustomTokenOption
    {
        public string Issuer { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public string SecurityKey { get; set; }
    }
}

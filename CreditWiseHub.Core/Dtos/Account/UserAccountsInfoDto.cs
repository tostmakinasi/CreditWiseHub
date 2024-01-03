namespace CreditWiseHub.Core.Dtos.Account
{
    public class UserAccountsInfoDto
    {
        public decimal TotalBalance { get; set; }

        public List<AccountInfoDto> UserAccounts { get; set; }
    }
}

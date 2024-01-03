namespace CreditWiseHub.Core.Dtos.Account
{
    public class AffectedAccountDto
    {
        public string AccountNumber { get; set; }
        public bool IsReceiverAccount { get; set; }
        public bool IsExternalAccount { get; set; }

        public string? BankName { get; set; }
        public string? OwnerFullName { get; set; }
    }
}

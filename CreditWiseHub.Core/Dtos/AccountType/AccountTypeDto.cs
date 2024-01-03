namespace CreditWiseHub.Core.Dtos.AccountType
{
    public class AccountTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MinimumOpeningBalance { get; set; }
        public string? Description { get; set; }
    }
}

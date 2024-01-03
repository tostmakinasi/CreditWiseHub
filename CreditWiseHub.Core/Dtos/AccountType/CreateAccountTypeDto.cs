namespace CreditWiseHub.Core.Dtos.AccountType
{
    public class CreateAccountTypeDto
    {
        public string Name { get; set; }
        public decimal MinimumOpeningBalance { get; set; }
        public string? Description { get; set; }
    }
}

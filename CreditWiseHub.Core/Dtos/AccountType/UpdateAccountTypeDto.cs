namespace CreditWiseHub.Core.Dtos.AccountType
{
    public class UpdateAccountTypeDto
    {
        public string? Name { get; set; }
        public decimal? MinimumOpeningBalance { get; set; }
        public string? Description { get; set; }
    }
}

namespace CreditWiseHub.Core.Dtos.Account
{
    public class CreateAccountDto
    {
        public string Name { get; set; }
        public int AccountTypeId { get; set; }
        public decimal OpeningBalance { get; set; }
        public string? Description { get; set; }
    }
}

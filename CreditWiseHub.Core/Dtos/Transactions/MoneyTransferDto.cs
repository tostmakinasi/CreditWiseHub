using CreditWiseHub.Core.Dtos.Account;

namespace CreditWiseHub.Core.Dtos.Transactions
{
    public class MoneyTransferDto
    {
        public decimal Amount { get; set; }
        public AffectedInBankAccountDto AccountInformation { get; set; }
    }
}

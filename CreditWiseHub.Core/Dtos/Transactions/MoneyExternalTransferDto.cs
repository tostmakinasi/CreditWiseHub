using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Enums;

namespace CreditWiseHub.Core.Dtos.Transactions
{
    public class MoneyExternalTransferDto
    {
        public MoneyTransferType TransferType { get; set; }
        public decimal Amount { get; set; }
        public AffectedExternalAccountDto AccountInformation { get; set; }
    }
}

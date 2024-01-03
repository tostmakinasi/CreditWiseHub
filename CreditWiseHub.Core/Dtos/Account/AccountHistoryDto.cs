using CreditWiseHub.Core.Dtos.Transactions;

namespace CreditWiseHub.Core.Dtos.Account
{
    public class AccountHistoryDto
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionDetailForAccountHistoryDto> AccountHistories { get; set; }
    }
}

namespace CreditWiseHub.Core.Dtos.Transactions
{
    public class TransactionDetailForAccountHistoryDto
    {
        public string Description { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal AfterTransactionBalance { get; set; }
        public decimal BeforeTransactionBalance { get; set; }
        public DateTime TransactionDateTime { get; set; }
    }
}

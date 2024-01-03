namespace CreditWiseHub.Core.Dtos.Transactions
{
    public class TransactionStatusDto
    {
        public long TransactionId { get; set; }
        public string TransactionStatus { get; set; }
        public TransactionStatusDto()
        {

        }

        public TransactionStatusDto(long transactionId, string transactionStatus)
        {
            TransactionId = transactionId;
            TransactionStatus = transactionStatus;
        }
    }
}

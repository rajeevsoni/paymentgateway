namespace BankSimulator.Models
{
    public class TransactionResponse
    {
        public Guid TransactionId { get; set; }
        public Guid MerchantId { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
    }
}

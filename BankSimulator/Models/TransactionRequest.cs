namespace BankSimulator.Models
{
    public class TransactionRequest
    {
        public Guid TransactionId { get; }

        public Guid MerchantId { get; }

        public string CardNumber { get; }

        public int ExpiryMonth { get; }

        public int ExpiryDate { get; }

        public string Name { get; }

        public decimal Amount { get; }

        public string CurrencyCode { get; }

        public int Cvv { get; }


        public TransactionRequest(
            Guid transactionId,
            Guid merchantId,
            string cardNumber,
            int expiryMonth,
            int expiryDate,
            string name,
            decimal amount,
            string currencyCode,
            int cvv)
        {
            TransactionId = transactionId;
            MerchantId = merchantId;
            CardNumber = cardNumber;
            ExpiryMonth = expiryMonth;
            ExpiryDate = expiryDate;
            Name = name;
            Amount = amount;
            CurrencyCode = currencyCode;
            Cvv = cvv;
        }
    }

    public enum TransactionStatus
    {
        pending,
        failed,
        success
    }
}

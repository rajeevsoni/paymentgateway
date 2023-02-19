namespace PaymentGateway.Data
{
    public class PaymentDetails
    {
        public Guid PaymentId { get; }

        public Guid  MerchantIdentifier { get; }

        public PaymentStatus PaymentStatus { get; private set; }

        public decimal Amount { get; }

        public string CurrencyCode { get; }

        public PaymentDetails(Guid paymentId, Guid merchantIdentifier, PaymentStatus paymentStatus, decimal amount, string currencyCode)
        {
            PaymentId = paymentId;
            MerchantIdentifier = merchantIdentifier;
            PaymentStatus = paymentStatus;
            Amount = amount;
            CurrencyCode = currencyCode;
        }

        public void UpdatePaymentStatus(PaymentStatus paymentStatus) 
        {
            this.PaymentStatus = paymentStatus;
        }
    }
}
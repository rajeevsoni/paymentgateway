namespace PaymentGateway.Data.Repository
{
    public interface IPaymentDetailsRepository
    {
        Task AddpaymentDetails(PaymentDetails paymentDetails);
        Task<PaymentDetails> GetPaymentDetails(Guid paymentId);
        Task UpdatepaymentDetails(PaymentDetails paymentDetails);
    }
}

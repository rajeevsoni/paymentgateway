using PaymentGateway.Data;
using PaymentGateway.Data.Models;

namespace PaymentGatewayAPI.Services
{
    public interface IPaymentGatewayService
    {
        Task<PaymentResponse> SubmitPaymentRequest(PaymentRequest paymentRequest);
        Task<PaymentDetails> GetPaymentDetails(Guid paymentId);
    }
}

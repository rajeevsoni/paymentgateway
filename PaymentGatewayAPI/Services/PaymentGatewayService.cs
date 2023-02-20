using Azure.Storage.Queues;
using Newtonsoft.Json;
using PaymentGateway.Data;
using PaymentGateway.Data.Models;
using PaymentGateway.Data.Repository;

namespace PaymentGatewayAPI.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly QueueClient _queueClient;
        private readonly IPaymentDetailsRepository _paymentDetailsRepository; 
        private readonly ILogger<PaymentGatewayService> _logger;

        public PaymentGatewayService(QueueClient queueClient, IPaymentDetailsRepository paymentDetailsRepository, ILogger<PaymentGatewayService> logger)
        {
            _queueClient = queueClient;
            _paymentDetailsRepository= paymentDetailsRepository;
            _logger = logger;
        }

        public async Task<PaymentDetails> GetPaymentDetails(Guid paymentId)
        {
            return await _paymentDetailsRepository.GetPaymentDetails(paymentId);
        }

        public async Task<PaymentResponse> SubmitPaymentRequest(PaymentRequest paymentRequest)
        {
            _logger.LogInformation($"In PaymentGateService");
            PaymentResponse  paymentResponse = new PaymentResponse();
            PaymentRequestQueueItem paymentRequestQueueItem = new PaymentRequestQueueItem();
            paymentRequestQueueItem.PaymentId = Guid.NewGuid();
            paymentRequestQueueItem.PaymentRequest = paymentRequest;

            try
            {
                string serializedRequest = JsonConvert.SerializeObject(paymentRequestQueueItem);
                await _queueClient.SendMessageAsync(serializedRequest);
                _logger.LogInformation($"Payment request submitted successfully for paymentId {paymentRequestQueueItem.PaymentId}");
            }
            catch
            {
                _logger.LogError($"Payment request failed to submit for paymentId {paymentRequestQueueItem.PaymentId}");
                throw;
            }

            paymentResponse.PaymentId = paymentRequestQueueItem.PaymentId.ToString();
            return paymentResponse;
        }
    }
}

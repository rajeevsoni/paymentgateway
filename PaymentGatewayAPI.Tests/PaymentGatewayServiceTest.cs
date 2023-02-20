using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Data;
using PaymentGateway.Data.Models;
using PaymentGateway.Data.Repository;
using PaymentGatewayAPI.Services;

namespace PaymentGatewayAPI.Tests
{
    [TestClass]
    public class PaymentGatewayServiceTest
    {
        private Mock<ILogger<PaymentGatewayService>> _logger;
        private Mock<QueueClient> _queueClient;
        private IPaymentDetailsRepository _paymentDetailsRepository;
        private IPaymentGatewayService _paymentGatewayService;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger<PaymentGatewayService>>();
            _queueClient = new Mock<QueueClient>();
            _paymentDetailsRepository = new PaymentDetailsRepository();
            _paymentGatewayService = new PaymentGatewayService(_queueClient.Object, _paymentDetailsRepository, _logger.Object);
        }

        [TestMethod]
        public async Task Validate_SubmitPaymentMethod()
        {
            //setup
            _queueClient.Setup(x => x.SendMessageAsync(It.IsAny<string>())).ReturnsAsync(It.IsAny<Response<SendReceipt>>);
            PaymentRequest paymentRequest = new PaymentRequest();
            paymentRequest.Cvv = 123;
            paymentRequest.Amount = 144;
            paymentRequest.CardNumber = "1234-3422-5666-3677";
            paymentRequest.ExpiryMonth = 12;
            paymentRequest.ExpiryYear = 2025;
            paymentRequest.Name = "User1";
            paymentRequest.CurrencyCode = "USD";

            var response = await _paymentGatewayService.SubmitPaymentRequest(paymentRequest);
            Assert.IsNotNull(response.PaymentId);
        }

        [TestMethod]
        public async Task Validate_GetPaymentDetails()
        {
            //setup
            _queueClient.Setup(x => x.SendMessageAsync(It.IsAny<string>())).ReturnsAsync(It.IsAny<Response<SendReceipt>>);
            PaymentDetails paymentDetails= new PaymentDetails(Guid.NewGuid(), Guid.NewGuid(), PaymentStatus.success, 123, "USD");
            await _paymentDetailsRepository.AddpaymentDetails(paymentDetails);

            PaymentDetails response = await _paymentGatewayService.GetPaymentDetails(paymentDetails.PaymentId);
            Assert.IsNotNull(paymentDetails);
            Assert.AreEqual(response.Amount, paymentDetails.Amount);
            Assert.AreEqual(response.CurrencyCode, paymentDetails.CurrencyCode);
        }
    }
}

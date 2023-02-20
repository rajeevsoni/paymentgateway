using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Data;
using PaymentGateway.Data.Models;
using PaymentGatewayAPI.Controllers;
using PaymentGatewayAPI.Services;

namespace PaymentGatewayAPI.Tests
{
    [TestClass]
    public class PaymentsControllerTest
    {
        private Mock<ILogger<PaymentsController>> _logger;
        private Mock<IPaymentGatewayService> _paymentGatewayService;
        private PaymentsController _paymentsController;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger<PaymentsController>>();
            _paymentGatewayService =  new Mock<IPaymentGatewayService>();
            _paymentsController = new PaymentsController(_paymentGatewayService.Object, _logger.Object);
        }

        [TestMethod]
        public async Task Test_PostPayment_ForAccepted()
        {
            PaymentResponse paymentResponse = new PaymentResponse();
            paymentResponse.PaymentId = Guid.NewGuid().ToString();
            _paymentGatewayService.Setup(x => x.SubmitPaymentRequest(It.IsAny<PaymentRequest>())).ReturnsAsync(paymentResponse);

            PaymentRequest paymentRequest = new PaymentRequest();
            paymentRequest.Cvv = 123;
            paymentRequest.Amount = 144;
            paymentRequest.CardNumber = "1234-3422-5666-3677";
            paymentRequest.ExpiryMonth = 12;
            paymentRequest.ExpiryYear = 2025;
            paymentRequest.Name = "User1";
            paymentRequest.CurrencyCode = "USD";

            var response = await _paymentsController.Post(paymentRequest);
            response.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod]
        public async Task Test_GetPayment_ForNotFound()
        {
            //setup
            PaymentDetails paymentDetails = null;
            _paymentGatewayService.Setup(x => x.GetPaymentDetails(It.IsAny<Guid>())).ReturnsAsync(paymentDetails);

            var response = await _paymentsController.Get(Guid.NewGuid().ToString());
            response.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task Test_GetPayment_ForOk()
        {
            //setup
            Guid paymentID = Guid.NewGuid();
            PaymentDetails paymentDetails = new PaymentDetails(paymentID, Guid.NewGuid(), PaymentStatus.success, 231, "USD");
            _paymentGatewayService.Setup(x => x.GetPaymentDetails(It.IsAny<Guid>())).ReturnsAsync(paymentDetails);

            var response = await _paymentsController.Get(paymentID.ToString());
            response.Should().BeOfType<OkObjectResult>();
        }
    }
}
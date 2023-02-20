using PaymentGateway.Data.Repository;

namespace PaymentGateway.Data.Tests
{
    [TestClass]
    public class PaymentDetailsRepositoryTest
    {
        private IPaymentDetailsRepository _paymentDetailsRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _paymentDetailsRepository = new PaymentDetailsRepository();
        }

        [TestMethod]
        public async Task Validate_AddPaymentDetailsWithGetPaymentDetails_NotNullResult()
        {
            PaymentDetails paymentDetails = new PaymentDetails(Guid.NewGuid(), Guid.NewGuid(), PaymentStatus.pending, 234, "USD");
            await _paymentDetailsRepository.AddpaymentDetails(paymentDetails);
            PaymentDetails fetchedPaymentDetails = await _paymentDetailsRepository.GetPaymentDetails(paymentDetails.PaymentId);

            Assert.IsNotNull(fetchedPaymentDetails);
            Assert.AreEqual(paymentDetails.PaymentId, fetchedPaymentDetails.PaymentId);
            Assert.AreEqual(paymentDetails.PaymentStatus, fetchedPaymentDetails.PaymentStatus);
            Assert.AreEqual(paymentDetails.Amount, fetchedPaymentDetails.Amount);
        }

        [TestMethod]
        public async Task Validate_UpdatePaymentDetailsWithGetPaymentDetails_NotNullResult()
        {
            PaymentDetails paymentDetails = new PaymentDetails(Guid.NewGuid(), Guid.NewGuid(), PaymentStatus.pending, 234, "USD");
            await _paymentDetailsRepository.AddpaymentDetails(paymentDetails);

            paymentDetails.UpdatePaymentStatus(PaymentStatus.success);

            PaymentDetails fetchedPaymentDetails = await _paymentDetailsRepository.GetPaymentDetails(paymentDetails.PaymentId);
            Assert.IsNotNull(fetchedPaymentDetails);
            Assert.AreEqual(paymentDetails.PaymentId, fetchedPaymentDetails.PaymentId);
            Assert.AreEqual(fetchedPaymentDetails.PaymentStatus, PaymentStatus.success);
            Assert.AreEqual(paymentDetails.Amount, fetchedPaymentDetails.Amount);
        }

        [TestMethod]
        public async Task Validate_AddPaymentDetailsWithGetPaymentDetails_NullResult()
        {
            PaymentDetails paymentDetails = new PaymentDetails(Guid.NewGuid(), Guid.NewGuid(), PaymentStatus.pending, 234, "USD");
            await _paymentDetailsRepository.AddpaymentDetails(paymentDetails);
            PaymentDetails fetchedPaymentDetails = await _paymentDetailsRepository.GetPaymentDetails(Guid.NewGuid());

            Assert.IsNull(fetchedPaymentDetails);
        }
    }
}
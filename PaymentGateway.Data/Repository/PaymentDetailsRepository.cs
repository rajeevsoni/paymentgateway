namespace PaymentGateway.Data.Repository
{
    public class PaymentDetailsRepository : IPaymentDetailsRepository
    {
        private static IList<PaymentDetails> _paymentDetailsList = new List<PaymentDetails>();

        public async Task AddpaymentDetails(PaymentDetails paymentDetails)
        {
            if (_paymentDetailsList.FirstOrDefault(x => x.PaymentId == paymentDetails.PaymentId) == null)
            {
                _paymentDetailsList.Add(paymentDetails);
            }
        }

        public async Task<PaymentDetails> GetPaymentDetails(Guid paymentId)
        {
            await Task.Delay(10);
            return _paymentDetailsList.SingleOrDefault(x => x.PaymentId.Equals(paymentId));
        }

        public async Task UpdatepaymentDetails(PaymentDetails updatedPaymentDetails)
        {
            PaymentDetails existingPaymentDetails = _paymentDetailsList.FirstOrDefault(x => x.PaymentId == updatedPaymentDetails.PaymentId);
            if (existingPaymentDetails != null)
            {
                existingPaymentDetails.UpdatePaymentStatus(updatedPaymentDetails.PaymentStatus);
            }
        }
    }
}

using PaymentGateway.Data.Models;

namespace PaymentGateway.Data
{
    public class PaymentRequestQueueItem
    {
        public Guid PaymentId { get; set; }
        public PaymentRequest PaymentRequest { get; set; }
    }
}

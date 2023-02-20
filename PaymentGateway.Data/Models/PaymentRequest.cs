using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Data.Models
{
    public class PaymentRequest
    {
        [Required]
        public string CardNumber { get; set; }

        [Required]
        public int ExpiryMonth { get; set; }

        [Required]
        public int ExpiryYear { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public int Cvv { get; set; }
    }
}

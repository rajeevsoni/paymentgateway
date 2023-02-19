using FluentValidation;

namespace PaymentGateway.Data.Models
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidator()
        {
            EnsureInstanceNotNull(this);

            var currentYear = DateTime.UtcNow.Year;

            RuleFor(r => r.CardNumber)
                .NotEmpty()
                .CreditCard();
            RuleFor(r => r.ExpiryMonth)
                .NotEmpty()
                .InclusiveBetween(1, 12);
            RuleFor(r => r.ExpiryDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(currentYear);
            RuleFor(r => r.Name)
                .NotEmpty();
            RuleFor(r => r.Amount)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.CurrencyCode)
                .Length(3)
                .NotEmpty();

            RuleFor(r => r.Cvv)
                .InclusiveBetween(100, 9999);
        }
    }
}

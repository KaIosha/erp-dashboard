using backend.dtos;
using FluentValidation;

namespace backend.Validation.Invoice
{
    public class PayInvoiceValidator : AbstractValidator<PayInvoiceDto>
    {
        public PayInvoiceValidator()
        {
            RuleFor(x => x.PaymentDate)
                .NotEmpty().WithMessage("Payment date is required.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .MaximumLength(50).WithMessage("Payment method must not exceed 50 characters.");
        }
    }
}
using backend.dtos;
using FluentValidation;

namespace backend.Validation.Invoice
{
    public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceDto>
    {
        public CreateInvoiceValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer is required.");

            RuleFor(x => x.InvoiceDate)
                .NotEmpty().WithMessage("Invoice date is required.");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required.")
                .GreaterThan(x => x.InvoiceDate)
                .WithMessage("Due date must be after the invoice date.");

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Order must be valid.");

            RuleFor(x => x.Lines)
                .NotEmpty().WithMessage("At least one line item is required.");

            RuleForEach(x => x.Lines)
                .SetValidator(new CreateInvoiceLineValidator());
        }
    }
}
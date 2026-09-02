using backend.dtos;
using FluentValidation;

namespace backend.Validation.Invoice
{
    public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceDto>
    {
        public UpdateInvoiceValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer must be valid.");

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Order must be valid.");

            When(x => x.InvoiceDate.HasValue && x.DueDate.HasValue, () =>
            {
                RuleFor(x => x.DueDate)
                    .GreaterThan(x => x.InvoiceDate!.Value)
                    .WithMessage("Due date must be after the invoice date.");
            });

            When(x => x.Lines is not null, () =>
            {
                RuleForEach(x => x.Lines)
                    .SetValidator(new UpdateInvoiceLineValidator());
            });
        }
    }
}
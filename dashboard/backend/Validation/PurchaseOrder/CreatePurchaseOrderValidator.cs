using backend.dtos;
using FluentValidation;

namespace backend.Validation.PurchaseOrder
{
    public class CreatePurchaseOrderValidator : AbstractValidator<CreatePurchaseOrderDto>
    {
        public CreatePurchaseOrderValidator()
        {
            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier is required.");

            RuleFor(x => x.ExpectedDelivery)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expected delivery must be in the future.");

            RuleFor(x => x.Lines)
                .NotEmpty().WithMessage("At least one line item is required.");

            RuleForEach(x => x.Lines)
                .SetValidator(new CreatePurchaseLineValidator());
        }
    }
}

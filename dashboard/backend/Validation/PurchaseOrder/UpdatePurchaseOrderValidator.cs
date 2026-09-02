using backend.dtos;
using FluentValidation;

namespace backend.Validation.PurchaseOrder
{
    public class UpdatePurchaseOrderValidator : AbstractValidator<UpdatePurchaseOrderDto>
    {
        public UpdatePurchaseOrderValidator()
        {
            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier must be valid.");

            RuleFor(x => x.ExpectedDelivery)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expected delivery must be in the future.");

            When(x => x.Lines is not null, () =>
            {
                RuleForEach(x => x.Lines)
                    .SetValidator(new UpdatePurchaseLineValidator());
            });
        }
    }
}

using backend.dtos;
using FluentValidation;

namespace backend.Validation.PurchaseOrder
{
    public class UpdatePurchaseLineValidator : AbstractValidator<UpdatePurchaseLineDto>
    {
        public UpdatePurchaseLineValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0).WithMessage("Line Id must be valid.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.UnitCost)
                .GreaterThanOrEqualTo(0).WithMessage("Unit cost cannot be negative.");
        }
    }
}

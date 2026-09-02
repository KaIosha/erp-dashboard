using backend.dtos;
using FluentValidation;

namespace backend.Validation.Order
{
    public class CreateOrderLineValidator : AbstractValidator<CreateOrderLineDto>
    {
        public CreateOrderLineValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative.");
        }
    }
}
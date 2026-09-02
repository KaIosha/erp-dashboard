using backend.dtos;
using FluentValidation;

namespace backend.Validation.Order
{
    public class UpdateOrderLineValidator : AbstractValidator<UpdateOrderLineDto>
    {
        public UpdateOrderLineValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0).WithMessage("Line Id must be valid.");

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
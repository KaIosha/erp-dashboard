using backend.dtos;
using FluentValidation;

namespace backend.Validation.Order
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer is required.");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Order date is required.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .MaximumLength(50).WithMessage("Payment method must not exceed 50 characters.");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required.")
                .MaximumLength(200).WithMessage("Shipping address must not exceed 200 characters.");

            RuleFor(x => x.Lines)
                .NotEmpty().WithMessage("At least one line item is required.");

            RuleForEach(x => x.Lines)
                .SetValidator(new CreateOrderLineValidator());
        }
    }
}
using backend.dtos;
using FluentValidation;

namespace backend.Validation.Order
{
    public class UpdateOrderValidator : AbstractValidator<UpdateOrderDto>
    {
        public UpdateOrderValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer must be valid.");

            RuleFor(x => x.PaymentMethod)
                .MaximumLength(50).WithMessage("Payment method must not exceed 50 characters.");

            RuleFor(x => x.ShippingAddress)
                .MaximumLength(200).WithMessage("Shipping address must not exceed 200 characters.");

            When(x => x.Lines is not null, () =>
            {
                RuleForEach(x => x.Lines)
                    .SetValidator(new UpdateOrderLineValidator());
            });
        }
    }
}
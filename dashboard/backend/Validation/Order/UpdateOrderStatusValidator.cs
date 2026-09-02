using backend.dtos;
using FluentValidation;

namespace backend.Validation.Order
{
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        private static readonly string[] AllowedStatuses =
            { "Pending", "Confirmed", "Shipped", "Delivered" };

        public UpdateOrderStatusValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage("Status must be one of: Pending, Confirmed, Shipped, Delivered.");
        }
    }
}
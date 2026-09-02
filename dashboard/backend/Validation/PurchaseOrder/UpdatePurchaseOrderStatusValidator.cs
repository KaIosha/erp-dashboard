using backend.dtos;
using FluentValidation;

namespace backend.Validation.PurchaseOrder
{
    public class UpdatePurchaseOrderStatusValidator : AbstractValidator<UpdatePurchaseOrderStatusDto>
    {
        private static readonly string[] AllowedStatuses =
            { "Pending", "Received", "Cancelled" };

        public UpdatePurchaseOrderStatusValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage("Status must be one of: Pending, Received, Cancelled.");
        }
    }
}

using backend.dtos;
using FluentValidation;

namespace backend.Validation.Product
{
    public class AdjustStockValidator : AbstractValidator<AdjustStockServiceFuncDto>
    {
        public AdjustStockValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product Id is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity change is required.");
        }
    }
}

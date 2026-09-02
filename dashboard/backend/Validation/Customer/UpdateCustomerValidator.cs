using backend.dtos;
using FluentValidation;

namespace backend.Validation.Customer
{
    public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDataDto>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(30).WithMessage("Phone must not exceed 30 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

            RuleFor(x => x.City)
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");
        }
    }
}

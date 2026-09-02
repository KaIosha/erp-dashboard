using backend.dtos;
using FluentValidation;

namespace backend.Validation.Supplier
{
    public class CreateSupplierValidator : AbstractValidator<CreateSupplierDto>
    {
        public CreateSupplierValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");

            RuleFor(x => x.ContactName)
                .NotEmpty().WithMessage("Contact name is required.")
                .MaximumLength(100).WithMessage("Contact name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MaximumLength(30).WithMessage("Phone must not exceed 30 characters.");

            RuleFor(x => x.PaymentTerms)
                .NotEmpty().WithMessage("Payment terms are required.")
                .MaximumLength(200).WithMessage("Payment terms must not exceed 200 characters.");
        }
    }
}

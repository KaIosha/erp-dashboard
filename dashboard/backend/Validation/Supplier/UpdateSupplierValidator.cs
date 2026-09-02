using backend.dtos;
using FluentValidation;

namespace backend.Validation.Supplier
{
    public class UpdateSupplierValidator : AbstractValidator<UpdateSupplierDto>
    {
        public UpdateSupplierValidator()
        {
            RuleFor(x => x.CompanyName)
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");

            RuleFor(x => x.ContactName)
                .MaximumLength(100).WithMessage("Contact name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Phone)
                .MaximumLength(30).WithMessage("Phone must not exceed 30 characters.");

            RuleFor(x => x.PaymentTerms)
                .MaximumLength(200).WithMessage("Payment terms must not exceed 200 characters.");
        }
    }
}

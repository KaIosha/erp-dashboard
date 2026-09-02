using backend.dtos;
using FluentValidation;

namespace backend.Validation.User
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDataDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
        }
    }
}

using backend.dtos;
using FluentValidation;

namespace backend.Validation.Role
{
    public class UpdateRoleValidator : AbstractValidator<UpdateRoleDataDto>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");

            When(x => x.Permissions is not null, () =>
            {
                RuleFor(x => x.Permissions)
                    .NotEmpty().WithMessage("Role permissions cannot be empty.");
            });
        }
    }
}

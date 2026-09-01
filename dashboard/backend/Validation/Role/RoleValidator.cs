using backend.dtos;
using FluentValidation;

namespace backend.Validation.Role
{
    public class RoleValidator : AbstractValidator<CreateRoleDto>
    {
        public RoleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");

            RuleFor(x=>x.Permissions)
                .NotEmpty().WithMessage("Role permissions are required.");
        }
    }
}

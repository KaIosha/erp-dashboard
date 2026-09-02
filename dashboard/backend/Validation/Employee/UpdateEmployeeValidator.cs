using backend.dtos;
using FluentValidation;

namespace backend.Validation.Employee
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
    {
        public UpdateEmployeeValidator()
        {
            When(x => !string.IsNullOrWhiteSpace(x.FirstName), () =>
            {
                RuleFor(x => x.FirstName)
                    .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.LastName), () =>
            {
                RuleFor(x => x.LastName)
                    .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Email must be a valid email address.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Phone), () =>
            {
                RuleFor(x => x.Phone)
                    .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Position), () =>
            {
                RuleFor(x => x.Position)
                    .MaximumLength(100).WithMessage("Position must not exceed 100 characters.");
            });

            When(x => x.DepartmentId.HasValue, () =>
            {
                RuleFor(x => x.DepartmentId)
                    .GreaterThan(0).WithMessage("Department must be valid.");
            });

            When(x => x.Salary.HasValue, () =>
            {
                RuleFor(x => x.Salary)
                    .GreaterThanOrEqualTo(0).WithMessage("Salary cannot be negative.");
            });
        }
    }
}
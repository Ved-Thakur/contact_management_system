using FluentValidation;
using ContactManager.Server.Dtos;

namespace ContactManager.Server.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .MinimumLength(3).WithMessage("Email must be at least 3 characters")
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters")
                .EmailAddress().WithMessage("Invalid email format");

            // Password validation
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .MaximumLength(20).WithMessage("Password cannot exceed 20 characters");
        }
    }
}
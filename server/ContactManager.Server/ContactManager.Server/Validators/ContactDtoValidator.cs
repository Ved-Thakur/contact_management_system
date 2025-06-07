using FluentValidation;
using ContactManager.Server.Dtos;

namespace ContactManager.Server.Validators
{
    public class ContactDtoValidator : AbstractValidator<ContactDto>
    {
        public ContactDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters")
                .MaximumLength(20).WithMessage("Name cannot exceed 20 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name can only contain letters and spaces");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .MinimumLength(3).WithMessage("Email must be at least 3 characters")
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Length(10).WithMessage("Phone must be exactly 10 digits")
                .Matches(@"^[0-9]+$").WithMessage("Phone can only contain numbers");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MinimumLength(3).WithMessage("Address must be at least 3 characters")
                .MaximumLength(100).WithMessage("Address cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\s,.-]+$").WithMessage("Address contains invalid characters");
        }
    }
}

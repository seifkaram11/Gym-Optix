using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

class LoginRequestValidator :AbstractValidator<RegisterRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The email is required!")
            .EmailAddress().WithMessage("Invalid email address format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("The password is required!")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}

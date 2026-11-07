using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class LoginRequestValidator : AbstractValidator<LoginDto>
{
    public LoginRequestValidator()
    {
        // don't give details when user is loging in
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

class RegisterRequestValidator:AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(_=>_.Email)
        .NotEmpty().WithMessage("the Email is required!")
        .EmailAddress().WithMessage("Invalid email address format");

        RuleFor(_=>_.Password)
        .NotEmpty().WithMessage("the Password is required!");

        RuleFor(_=>_.PersonName)
        .NotEmpty().WithMessage("the Name is required!")
        .Length(1, 50).WithMessage("Person Name should be 1 to 50 characters long");

        RuleFor(_=>_.Gender)
        .IsInEnum().WithMessage("invalid value");
    }
}

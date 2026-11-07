using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        // 1. Username (Matches Identity's default AllowedUserNameCharacters)
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters long.")
            .Matches(@"^[a-zA-Z0-9-._@+]+$")
            .WithMessage("Username can only contain letters, numbers, and - . _ @ +");

        // 2. Email (Format check only)
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");

        // 3. Password (Matches Identity's default PasswordOptions)
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter ('A'-'Z').")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter ('a'-'z').")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit ('0'-'9').")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one non-alphanumeric character.");

        // 4. Gender
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value.");
    }
}
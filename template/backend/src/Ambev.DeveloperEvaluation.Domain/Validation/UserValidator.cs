using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email address format.");

        RuleFor(user => user.Username)
            .NotEmpty()
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username cannot be longer than 50 characters.");

        RuleFor(user => user.PasswordHash)
            .NotEmpty()
            .WithMessage("Password hash cannot be empty.");

        RuleFor(user => user.Phone)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{10,14}$")
            .WithMessage("Phone number must be in international format with 11-15 digits.");

        RuleFor(user => user.Status)
            .IsInEnum()
            .NotEqual(UserStatus.Unknown)
            .WithMessage("User status must be a valid value.");

        RuleFor(user => user.Role)
            .IsInEnum()
            .NotEqual(UserRole.None)
            .WithMessage("User role must be a valid value.");

        RuleFor(user => user.CreatedAt)
            .NotEmpty()
            .WithMessage("Creation date must be set.");
    }
}

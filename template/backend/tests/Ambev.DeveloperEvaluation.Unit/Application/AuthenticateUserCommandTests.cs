using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="AuthenticateUserCommand"/> class.
/// </summary>
public class AuthenticateUserCommandTests
{
    private readonly AuthenticateUserCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateUserCommandTests"/> class.
    /// </summary>
    public AuthenticateUserCommandTests()
    {
        _validator = new AuthenticateUserCommandValidator();
    }

    /// <summary>
    /// Tests that validation passes when all command properties are valid.
    /// </summary>
    [Fact(DisplayName = "Given valid command When validating Then should pass validation")]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Given
        var command = new AuthenticateUserCommand
        {
            Email = "test@example.com",
            Password = "Test@123"
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that validation fails when email is empty.
    /// </summary>
    [Fact(DisplayName = "Given empty email When validating Then should fail validation")]
    public void Validate_EmptyEmail_ShouldFail()
    {
        // Given
        var command = new AuthenticateUserCommand
        {
            Email = string.Empty,
            Password = "Test@123"
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    /// <summary>
    /// Tests that validation fails when email is invalid.
    /// </summary>
    [Fact(DisplayName = "Given invalid email When validating Then should fail validation")]
    public void Validate_InvalidEmail_ShouldFail()
    {
        // Given
        var command = new AuthenticateUserCommand
        {
            Email = "invalid-email",
            Password = "Test@123"
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    /// <summary>
    /// Tests that validation fails when password is empty.
    /// </summary>
    [Fact(DisplayName = "Given empty password When validating Then should fail validation")]
    public void Validate_EmptyPassword_ShouldFail()
    {
        // Given
        var command = new AuthenticateUserCommand
        {
            Email = "test@example.com",
            Password = string.Empty
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    /// <summary>
    /// Tests that validation fails when password is too short.
    /// </summary>
    [Fact(DisplayName = "Given short password When validating Then should fail validation")]
    public void Validate_ShortPassword_ShouldFail()
    {
        // Given
        var command = new AuthenticateUserCommand
        {
            Email = "test@example.com",
            Password = "Test@1"
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
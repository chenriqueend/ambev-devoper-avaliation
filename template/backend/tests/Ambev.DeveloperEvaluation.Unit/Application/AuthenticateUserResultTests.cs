using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="AuthenticateUserResult"/> class.
/// </summary>
public class AuthenticateUserResultTests
{
    /// <summary>
    /// Tests that a successful authentication result is created correctly.
    /// </summary>
    [Fact(DisplayName = "Given successful authentication When creating result Then should have success properties")]
    public void Create_SuccessfulAuthentication_ShouldHaveSuccessProperties()
    {
        // Given
        var userId = Guid.NewGuid();
        const string username = "testuser";
        const string role = "Customer";
        const string token = "jwt_token";

        // When
        var result = new AuthenticateUserResult
        {
            Success = true,
            Id = userId,
            Name = username,
            Role = role,
            Token = token
        };

        // Then
        result.Success.Should().BeTrue();
        result.Id.Should().Be(userId);
        result.Name.Should().Be(username);
        result.Role.Should().Be(role);
        result.Token.Should().Be(token);
        result.Message.Should().BeNull();
    }

    /// <summary>
    /// Tests that a failed authentication result is created correctly.
    /// </summary>
    [Fact(DisplayName = "Given failed authentication When creating result Then should have failure properties")]
    public void Create_FailedAuthentication_ShouldHaveFailureProperties()
    {
        // Given
        const string errorMessage = "Invalid credentials";

        // When
        var result = new AuthenticateUserResult
        {
            Success = false,
            Message = errorMessage
        };

        // Then
        result.Success.Should().BeFalse();
        result.Message.Should().Be(errorMessage);
        result.Id.Should().Be(Guid.Empty);
        result.Name.Should().BeEmpty();
        result.Role.Should().BeEmpty();
        result.Token.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that a suspended user authentication result is created correctly.
    /// </summary>
    [Fact(DisplayName = "Given suspended user When creating result Then should have suspended error")]
    public void Create_SuspendedUser_ShouldHaveSuspendedError()
    {
        // Given
        const string errorMessage = "User is suspended";

        // When
        var result = new AuthenticateUserResult
        {
            Success = false,
            Message = errorMessage
        };

        // Then
        result.Success.Should().BeFalse();
        result.Message.Should().Be(errorMessage);
        result.Id.Should().Be(Guid.Empty);
        result.Name.Should().BeEmpty();
        result.Role.Should().BeEmpty();
        result.Token.Should().BeEmpty();
    }
}
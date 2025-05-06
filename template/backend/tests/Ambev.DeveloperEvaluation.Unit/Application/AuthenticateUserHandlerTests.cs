using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.TestHelpers;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="AuthenticateUserHandler"/> class.
/// </summary>
public class AuthenticateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IMapper _mapper;
    private readonly AuthenticateUserHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateUserHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public AuthenticateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _tokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _mapper = Substitute.For<IMapper>();
        _handler = new AuthenticateUserHandler(_userRepository, _passwordHasher, _tokenGenerator);
    }

    /// <summary>
    /// Tests that a valid authentication request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid credentials When authenticating Then returns success response")]
    public async Task Handle_ValidCredentials_ReturnsSuccessResponse()
    {
        // Given
        var command = new AuthenticateUserCommand
        {
            Email = "test@example.com",
            Password = "Test@123"
        };

        var userId = Guid.NewGuid();
        var user = new User
        {
            Email = command.Email,
            PasswordHash = "hashedPassword",
            Username = "testuser",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };
        user.SetId(userId);

        var result = new AuthenticateUserResult
        {
            Success = true,
            Token = "jwt_token",
            Id = userId,
            Email = user.Email,
            Name = user.Username,
            Role = user.Role.ToString()
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.PasswordHash)
            .Returns(true);
        _tokenGenerator.GenerateToken(user.Id, user.Email)
            .Returns("jwt_token");

        // When
        var authResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        authResult.Should().NotBeNull();
        authResult.Success.Should().BeTrue();
        authResult.Token.Should().Be("jwt_token");
        authResult.Id.Should().Be(userId);
        authResult.Name.Should().Be(user.Username);
        authResult.Role.Should().Be(user.Role.ToString());
    }

    /// <summary>
    /// Tests that authentication fails when user is not found.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user When authenticating Then returns failure response")]
    public async Task Handle_UserNotFound_ReturnsFailureResponse()
    {
        // Arrange
        var command = new AuthenticateUserCommand
        {
            Email = "nonexistent@email.com",
            Password = "password123"
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    /// <summary>
    /// Tests that authentication fails when password is incorrect.
    /// </summary>
    [Fact(DisplayName = "Given incorrect password When authenticating Then returns failure response")]
    public async Task Handle_IncorrectPassword_ReturnsFailureResponse()
    {
        // Arrange
        var command = new AuthenticateUserCommand
        {
            Email = "test@email.com",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Email = command.Email,
            PasswordHash = "hashedPassword",
            Username = "testuser",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.PasswordHash)
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid password");
    }

    /// <summary>
    /// Tests that authentication fails when user is suspended.
    /// </summary>
    [Fact(DisplayName = "Given suspended user When authenticating Then returns failure response")]
    public async Task Handle_SuspendedUser_ReturnsFailureResponse()
    {
        // Given
        var command = new AuthenticateUserCommand
        {
            Email = "test@example.com",
            Password = "Test@123"
        };

        var user = new User
        {
            Email = command.Email,
            PasswordHash = "hashedPassword",
            Username = "testuser",
            Role = UserRole.Customer,
            Status = UserStatus.Suspended
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.PasswordHash)
            .Returns(true);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User is suspended");
    }
}
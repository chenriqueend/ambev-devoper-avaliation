using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Controllers;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Controllers;

/// <summary>
/// Contains unit tests for the <see cref="AuthController"/> class.
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthController _controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthControllerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _controller = new AuthController(_mediatorMock.Object, _mapperMock.Object);
    }

    /// <summary>
    /// Tests that a valid authentication request returns OK with token.
    /// </summary>
    [Fact(DisplayName = "Given valid credentials When authenticating Then returns OK with token")]
    public async Task AuthenticateUser_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = new AuthenticateUserRequest
        {
            Email = "test@email.com",
            Password = "password123"
        };

        var command = new AuthenticateUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = new AuthenticateUserResult
        {
            Success = true,
            Token = "valid-token",
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = "Test User",
            Role = "User"
        };

        var response = new AuthenticateUserResponse
        {
            Token = result.Token,
            Id = result.Id,
            Email = result.Email,
            Name = result.Name,
            Role = result.Role
        };

        _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request))
            .Returns(command);

        _mediatorMock.Setup(m => m.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        _mapperMock.Setup(m => m.Map<AuthenticateUserResponse>(result))
            .Returns(response);

        // Act
        var actionResult = await _controller.AuthenticateUser(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var apiResponse = Assert.IsType<ApiResponseWithData<AuthenticateUserResponse>>(okResult.Value);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(response.Token, apiResponse.Data.Token);
        Assert.Equal(response.Id, apiResponse.Data.Id);
        Assert.Equal(response.Email, apiResponse.Data.Email);
        Assert.Equal(response.Name, apiResponse.Data.Name);
        Assert.Equal(response.Role, apiResponse.Data.Role);
    }

    /// <summary>
    /// Tests that invalid credentials return Unauthorized.
    /// </summary>
    [Fact(DisplayName = "Given invalid credentials When authenticating Then returns Unauthorized")]
    public async Task AuthenticateUser_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthenticateUserRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword@123"
        };

        var command = new AuthenticateUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = new AuthenticateUserResult
        {
            Success = false,
            Message = "Invalid credentials"
        };

        _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request))
            .Returns(command);

        _mediatorMock.Setup(m => m.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.AuthenticateUser(request, CancellationToken.None);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
        Assert.False(apiResponse.Success);
        Assert.Equal("Invalid credentials", apiResponse.Message);
    }

    /// <summary>
    /// Tests that invalid request returns BadRequest.
    /// </summary>
    [Fact(DisplayName = "Given invalid request When authenticating Then returns BadRequest")]
    public async Task AuthenticateUser_InvalidRequest_ReturnsBadRequest()
    {
        // Given
        var request = new AuthenticateUserRequest
        {
            Email = "invalid-email",
            Password = ""
        };

        // When
        var actionResult = await _controller.AuthenticateUser(request, CancellationToken.None);

        // Then
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    /// <summary>
    /// Tests that suspended user returns Unauthorized.
    /// </summary>
    [Fact(DisplayName = "Given suspended user When authenticating Then returns Unauthorized")]
    public async Task AuthenticateUser_SuspendedUser_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthenticateUserRequest
        {
            Email = "test@example.com",
            Password = "Test@123"
        };

        var command = new AuthenticateUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = new AuthenticateUserResult
        {
            Success = false,
            Message = "User is suspended"
        };

        _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request))
            .Returns(command);

        _mediatorMock.Setup(m => m.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.AuthenticateUser(request, CancellationToken.None);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
        Assert.False(apiResponse.Success);
        Assert.Equal("User is suspended", apiResponse.Message);
    }
}
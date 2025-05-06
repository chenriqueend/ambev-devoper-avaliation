using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new AuthController(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task AuthenticateUser_WithValidCredentials_ShouldReturnOkWithToken()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "test@example.com",
                Password = "validPassword"
            };

            var command = new AuthenticateUserCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = new AuthenticateUserResult
            {
                Success = true,
                Token = "valid.jwt.token",
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

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            _mapperMock.Setup(m => m.Map<AuthenticateUserResponse>(result))
                .Returns(response);

            // Act
            var actionResult = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var apiResponse = okResult.Value.Should().BeOfType<ApiResponseWithData<AuthenticateUserResponse>>().Subject;
            apiResponse.Data.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task AuthenticateUser_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "test@example.com",
                Password = "invalidPassword"
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

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            var unauthorizedResult = actionResult.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            var apiResponse = unauthorizedResult.Value.Should().BeOfType<ApiResponse>().Subject;
            apiResponse.Message.Should().Be("Authentication failed");
        }

        [Fact]
        public async Task AuthenticateUser_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "invalid-email",
                Password = ""
            };

            // Act
            var actionResult = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>();
        }
    }
} 
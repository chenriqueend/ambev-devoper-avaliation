using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Application.Commands.Users;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Controllers;
using Ambev.DeveloperEvaluation.WebApi.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _controller = new UsersController(_mediatorMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User("user1@email.com", "password1") { Id = Guid.NewGuid() },
                new User("user2@email.com", "password2") { Id = Guid.NewGuid() }
            };

            _userRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<GetUserResponse>>(okResult.Value);
            var userList = response.ToList();
            Assert.Equal(2, userList.Count);
            Assert.Equal(users[0].Email, userList[0].Email);
            Assert.Equal(users[1].Email, userList[1].Email);
        }

        [Fact]
        public async Task GetAll_WhenRepositoryThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Database error", badRequestResult.Value.ToString());
        }

        // ... existing code ...
    }
} 
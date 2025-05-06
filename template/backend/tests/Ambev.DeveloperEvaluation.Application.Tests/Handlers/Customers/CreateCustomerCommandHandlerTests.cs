using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Commands.Customers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Application.Tests.Handlers.Customers
{
    public class CreateCustomerCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateCustomerCommandHandler _handler;

        public CreateCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateCustomerCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateCustomerSuccessfully()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                Name = "Test Customer",
                Email = "test@email.com",
                Phone = "1234567890"
            };

            _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(command.Name);
            result.Value.Email.Should().Be(command.Email);
            result.Value.Phone.Should().Be(command.Phone);

            _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                Name = string.Empty,
                Email = "test@email.com",
                Phone = "1234567890"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                Name = "Test Customer",
                Email = "test@email.com",
                Phone = "1234567890"
            };

            _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
} 
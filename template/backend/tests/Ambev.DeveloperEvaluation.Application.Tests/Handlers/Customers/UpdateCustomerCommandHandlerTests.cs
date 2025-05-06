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
    public class UpdateCustomerCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateCustomerCommandHandler _handler;

        public UpdateCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateCustomerCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldUpdateCustomerSuccessfully()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingCustomer = new Customer("Old Name", "old@email.com", "1234567890")
            {
                Id = customerId
            };

            var command = new UpdateCustomerCommand
            {
                Id = customerId,
                Name = "New Name",
                Email = "new@email.com",
                Phone = "0987654321"
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            _customerRepositoryMock.Verify(r => r.GetByIdAsync(customerId), Times.Once);
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Customer>(c => 
                c.Id == customerId && 
                c.Name == command.Name && 
                c.Email == command.Email && 
                c.Phone == command.Phone)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingCustomer_ShouldReturnFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new UpdateCustomerCommand
            {
                Id = customerId,
                Name = "New Name",
                Email = "new@email.com",
                Phone = "0987654321"
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _customerRepositoryMock.Verify(r => r.GetByIdAsync(customerId), Times.Once);
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldReturnFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new UpdateCustomerCommand
            {
                Id = customerId,
                Name = string.Empty,
                Email = "new@email.com",
                Phone = "0987654321"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _customerRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingCustomer = new Customer("Old Name", "old@email.com", "1234567890")
            {
                Id = customerId
            };

            var command = new UpdateCustomerCommand
            {
                Id = customerId,
                Name = "New Name",
                Email = "new@email.com",
                Phone = "0987654321"
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
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
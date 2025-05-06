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
    public class DeleteCustomerCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteCustomerCommandHandler _handler;

        public DeleteCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteCustomerCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldDeleteCustomerSuccessfully()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingCustomer = new Customer("Test Customer", "test@email.com", "1234567890")
            {
                Id = customerId
            };

            var command = new DeleteCustomerCommand
            {
                Id = customerId
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Customer>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            _customerRepositoryMock.Verify(r => r.GetByIdAsync(customerId), Times.Once);
            _customerRepositoryMock.Verify(r => r.DeleteAsync(It.Is<Customer>(c => c.Id == customerId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingCustomer_ShouldReturnFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new DeleteCustomerCommand
            {
                Id = customerId
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _customerRepositoryMock.Verify(r => r.GetByIdAsync(customerId), Times.Once);
            _customerRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Customer>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingCustomer = new Customer("Test Customer", "test@email.com", "1234567890")
            {
                Id = customerId
            };

            var command = new DeleteCustomerCommand
            {
                Id = customerId
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Customer>()))
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
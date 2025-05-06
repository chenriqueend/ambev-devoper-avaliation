using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Application.Handlers.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Application.Tests.Handlers.Sales
{
    public class CreateSaleCommandHandlerTests
    {
        private readonly Mock<ISaleRepository> _saleRepositoryMock;
        private readonly CreateSaleCommandHandler _handler;

        public CreateSaleCommandHandlerTests()
        {
            _saleRepositoryMock = new Mock<ISaleRepository>();
            _handler = new CreateSaleCommandHandler(_saleRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateSaleSuccessfully()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand
                    {
                        ProductId = Guid.NewGuid(),
                        ProductDescription = "Test Product",
                        Quantity = 2,
                        UnitPrice = 10.0m
                    }
                }
            };

            _saleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Sale>()))
                .Returns(Task.CompletedTask);

            _saleRepositoryMock
                .Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.SaleId.Should().NotBe(Guid.Empty);

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Once);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyItems_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<CreateSaleItemCommand>()
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().Be("Sale must have at least one item");

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Never);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WithNullItems_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = null
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().Be("Sale must have at least one item");

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Never);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WithRepositoryError_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand
                    {
                        ProductId = Guid.NewGuid(),
                        ProductDescription = "Test Product",
                        Quantity = 2,
                        UnitPrice = 10.0m
                    }
                }
            };

            _saleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Sale>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().Be("An error occurred while creating the sale");

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Once);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WithCommitError_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand
                    {
                        ProductId = Guid.NewGuid(),
                        ProductDescription = "Test Product",
                        Quantity = 2,
                        UnitPrice = 10.0m
                    }
                }
            };

            _saleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Sale>()))
                .Returns(Task.CompletedTask);

            _saleRepositoryMock
                .Setup(x => x.CommitAsync())
                .ThrowsAsync(new Exception("Commit error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().Be("An error occurred while saving the sale");

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Once);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }
    }
} 
using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Controllers;
using Ambev.DeveloperEvaluation.WebApi.Models.Sales;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Controllers
{
    public class SalesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SalesController _controller;

        public SalesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new SalesController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Create_WithValidRequest_ShouldReturnCreatedSale()
        {
            // Arrange
            var request = new SaleRequest
            {
                SaleNumber = "SALE001",
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                BranchId = Guid.NewGuid(),
                BranchName = "Test Branch",
                Items = new List<SaleItemRequest>
                {
                    new SaleItemRequest
                    {
                        ProductId = Guid.NewGuid(),
                        ProductDescription = "Test Product",
                        Quantity = 2,
                        UnitPrice = 10.0m
                    }
                }
            };

            var saleId = Guid.NewGuid();
            var commandResult = Result<Sale>.Success(new Sale(
                request.SaleNumber,
                request.CustomerId,
                request.CustomerName,
                request.BranchId,
                request.BranchName,
                request.Items.Select(i => new SaleItem(
                    i.ProductId,
                    i.ProductDescription,
                    i.Quantity,
                    i.UnitPrice
                )).ToList()
            ) { Id = saleId });

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateSaleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtResult = result as CreatedAtActionResult;
            createdAtResult.Value.Should().BeOfType<SaleResponse>();
            var response = createdAtResult.Value as SaleResponse;
            response.Id.Should().Be(saleId);
            response.SaleNumber.Should().Be(request.SaleNumber);
            response.CustomerId.Should().Be(request.CustomerId);
            response.CustomerName.Should().Be(request.CustomerName);
            response.BranchId.Should().Be(request.BranchId);
            response.BranchName.Should().Be(request.BranchName);
            response.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task Create_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SaleRequest
            {
                SaleNumber = string.Empty, // Invalid
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                BranchId = Guid.NewGuid(),
                BranchName = "Test Branch",
                Items = new List<SaleItemRequest>()
            };

            var commandResult = Result<Sale>.Failure("Invalid sale number");

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateSaleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(commandResult.Error);
        }

        [Fact]
        public async Task GetById_WithExistingSale_ShouldReturnSale()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var sale = new Sale(
                "SALE001",
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch",
                new List<SaleItem>()
            ) { Id = saleId };

            var commandResult = Result<Sale>.Success(sale);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSaleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.GetById(saleId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeOfType<SaleResponse>();
            var response = okResult.Value as SaleResponse;
            response.Id.Should().Be(saleId);
        }

        [Fact]
        public async Task GetById_WithNonExistingSale_ShouldReturnNotFound()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var commandResult = Result<Sale>.Failure("Sale not found");

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSaleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.GetById(saleId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be(commandResult.Error);
        }

        [Fact]
        public async Task UpdateItemQuantity_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var newQuantity = 5;

            var commandResult = Result<bool>.Success(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSaleItemQuantityCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.UpdateItemQuantity(saleId, productId, newQuantity);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateItemQuantity_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var newQuantity = -1; // Invalid quantity

            var commandResult = Result<bool>.Failure("Invalid quantity");

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSaleItemQuantityCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.UpdateItemQuantity(saleId, productId, newQuantity);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(commandResult.Error);
        }

        [Fact]
        public async Task CancelItem_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var commandResult = Result<bool>.Success(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelSaleItemCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CancelItem(saleId, productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task CancelItem_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var commandResult = Result<bool>.Failure("Item not found");

            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelSaleItemCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CancelItem(saleId, productId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(commandResult.Error);
        }

        [Fact]
        public async Task Cancel_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var saleId = Guid.NewGuid();

            var commandResult = Result<bool>.Success(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelSaleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.Cancel(saleId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Cancel_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var saleId = Guid.NewGuid();

            var commandResult = Result<bool>.Failure("Sale not found");

            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelSaleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.Cancel(saleId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(commandResult.Error);
        }
    }
} 
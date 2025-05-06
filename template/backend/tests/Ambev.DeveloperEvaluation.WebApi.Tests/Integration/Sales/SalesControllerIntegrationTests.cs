using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Integration.Sales
{
    public class SalesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly Mock<ISaleRepository> _saleRepositoryMock;

        public SalesControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _saleRepositoryMock = new Mock<ISaleRepository>();
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped(_ => _saleRepositoryMock.Object);
                });
            });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateSale_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new[]
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

            var saleId = Guid.NewGuid();
            _saleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Sale>()))
                .Returns(Task.CompletedTask);

            _saleRepositoryMock
                .Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);

            // Act
            var response = await _client.PostAsync(
                "/api/Sales",
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CreateSaleResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.SaleId.Should().NotBe(Guid.Empty);

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Once);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateSale_WithEmptyItems_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = Array.Empty<CreateSaleItemCommand>()
            };

            // Act
            var response = await _client.PostAsync(
                "/api/Sales",
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Sale must have at least one item");

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Never);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateSale_WithNullItems_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = null
            };

            // Act
            var response = await _client.PostAsync(
                "/api/Sales",
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Sale must have at least one item");

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Never);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateSale_WithRepositoryError_ShouldReturnInternalServerError()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new[]
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
            var response = await _client.PostAsync(
                "/api/Sales",
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("An error occurred while creating the sale");

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Once);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateSale_WithCommitError_ShouldReturnInternalServerError()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new[]
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
            var response = await _client.PostAsync(
                "/api/Sales",
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("An error occurred while saving the sale");

            _saleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Sale>()), Times.Once);
            _saleRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }
    }

    public class CreateSaleResponse
    {
        public bool Success { get; set; }
        public Guid SaleId { get; set; }
        public string Error { get; set; }
    }
} 
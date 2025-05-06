using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.WebApi.Models.Sales;
using Ambev.DeveloperEvaluation.WebApi.Tests.Integration;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Integration.Sales
{
    public class SalesControllerTests : TestBase
    {
        private readonly HttpClient _client;

        public SalesControllerTests()
        {
            _client = CreateClient();
        }

        [Fact]
        public async Task CreateSale_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var request = new SaleRequest
            {
                SaleNumber = "SALE-001",
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
                        Quantity = 5,
                        UnitPrice = 10.0m
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/sales", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var sale = await response.Content.ReadFromJsonAsync<SaleResponse>();
            Assert.NotNull(sale);
            Assert.Equal(request.SaleNumber, sale.SaleNumber);
            Assert.Equal(request.CustomerId, sale.CustomerId);
            Assert.Equal(request.CustomerName, sale.CustomerName);
            Assert.Equal(request.BranchId, sale.BranchId);
            Assert.Equal(request.BranchName, sale.BranchName);
            Assert.Single(sale.Items);
            Assert.Equal(45.0m, sale.TotalAmount); // 5 * 10.0 * 0.9 (10% discount)
        }

        [Fact]
        public async Task CreateSale_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SaleRequest
            {
                SaleNumber = "", // Invalid sale number
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                BranchId = Guid.NewGuid(),
                BranchName = "Test Branch",
                Items = new List<SaleItemRequest>()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/sales", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSale_WithValidId_ShouldReturnSale()
        {
            // Arrange
            var createRequest = new SaleRequest
            {
                SaleNumber = "SALE-002",
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
                        Quantity = 5,
                        UnitPrice = 10.0m
                    }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
            var createdSale = await createResponse.Content.ReadFromJsonAsync<SaleResponse>();

            // Act
            var response = await _client.GetAsync($"/api/sales/{createdSale.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var sale = await response.Content.ReadFromJsonAsync<SaleResponse>();
            Assert.NotNull(sale);
            Assert.Equal(createdSale.Id, sale.Id);
            Assert.Equal(createRequest.SaleNumber, sale.SaleNumber);
        }

        [Fact]
        public async Task GetSale_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync($"/api/sales/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateItemQuantity_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var createRequest = new SaleRequest
            {
                SaleNumber = "SALE-003",
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
                        Quantity = 5,
                        UnitPrice = 10.0m
                    }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
            var createdSale = await createResponse.Content.ReadFromJsonAsync<SaleResponse>();
            var productId = createdSale.Items[0].ProductId;

            // Act
            var response = await _client.PutAsync(
                $"/api/sales/{createdSale.Id}/items/{productId}/quantity?newQuantity=15",
                null);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/sales/{createdSale.Id}");
            var updatedSale = await getResponse.Content.ReadFromJsonAsync<SaleResponse>();
            Assert.Equal(15, updatedSale.Items[0].Quantity);
            Assert.Equal(120.0m, updatedSale.TotalAmount); // 15 * 10.0 * 0.8 (20% discount)
        }

        [Fact]
        public async Task CancelItem_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var createRequest = new SaleRequest
            {
                SaleNumber = "SALE-004",
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
                        Quantity = 5,
                        UnitPrice = 10.0m
                    }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
            var createdSale = await createResponse.Content.ReadFromJsonAsync<SaleResponse>();
            var productId = createdSale.Items[0].ProductId;

            // Act
            var response = await _client.DeleteAsync(
                $"/api/sales/{createdSale.Id}/items/{productId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the cancellation
            var getResponse = await _client.GetAsync($"/api/sales/{createdSale.Id}");
            var updatedSale = await getResponse.Content.ReadFromJsonAsync<SaleResponse>();
            Assert.True(updatedSale.Items[0].IsCancelled);
            Assert.Equal(0, updatedSale.TotalAmount);
        }

        [Fact]
        public async Task CancelSale_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var createRequest = new SaleRequest
            {
                SaleNumber = "SALE-005",
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
                        Quantity = 5,
                        UnitPrice = 10.0m
                    }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
            var createdSale = await createResponse.Content.ReadFromJsonAsync<SaleResponse>();

            // Act
            var response = await _client.DeleteAsync($"/api/sales/{createdSale.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the cancellation
            var getResponse = await _client.GetAsync($"/api/sales/{createdSale.Id}");
            var updatedSale = await getResponse.Content.ReadFromJsonAsync<SaleResponse>();
            Assert.True(updatedSale.IsCancelled);
            Assert.Equal(0, updatedSale.TotalAmount);
        }
    }
} 
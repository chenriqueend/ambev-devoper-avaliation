using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.WebApi.Models.Customers;
using Ambev.DeveloperEvaluation.WebApi.Tests.Integration;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Integration.Customers
{
    public class CustomersControllerTests : TestBase
    {
        private readonly HttpClient _client;

        public CustomersControllerTests()
        {
            _client = CreateClient();
        }

        [Fact]
        public async Task CreateCustomer_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var request = new CustomerRequest
            {
                Name = "Test Customer",
                Email = "test@example.com",
                Phone = "1234567890"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/customers", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var customer = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            Assert.NotNull(customer);
            Assert.Equal(request.Name, customer.Name);
            Assert.Equal(request.Email, customer.Email);
            Assert.Equal(request.Phone, customer.Phone);
        }

        [Fact]
        public async Task CreateCustomer_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CustomerRequest
            {
                Name = "", // Invalid name
                Email = "test@example.com",
                Phone = "1234567890"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/customers", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCustomer_WithValidId_ShouldReturnCustomer()
        {
            // Arrange
            var createRequest = new CustomerRequest
            {
                Name = "Test Customer",
                Email = "test@example.com",
                Phone = "1234567890"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/customers", createRequest);
            var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            // Act
            var response = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var customer = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            Assert.NotNull(customer);
            Assert.Equal(createdCustomer.Id, customer.Id);
            Assert.Equal(createRequest.Name, customer.Name);
        }

        [Fact]
        public async Task GetCustomer_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCustomer_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var createRequest = new CustomerRequest
            {
                Name = "Test Customer",
                Email = "test@example.com",
                Phone = "1234567890"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/customers", createRequest);
            var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            var updateRequest = new CustomerRequest
            {
                Name = "Updated Customer",
                Email = "updated@example.com",
                Phone = "0987654321"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/customers/{createdCustomer.Id}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
            var updatedCustomer = await getResponse.Content.ReadFromJsonAsync<CustomerResponse>();
            Assert.Equal(updateRequest.Name, updatedCustomer.Name);
            Assert.Equal(updateRequest.Email, updatedCustomer.Email);
            Assert.Equal(updateRequest.Phone, updatedCustomer.Phone);
        }

        [Fact]
        public async Task DeleteCustomer_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var createRequest = new CustomerRequest
            {
                Name = "Test Customer",
                Email = "test@example.com",
                Phone = "1234567890"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/customers", createRequest);
            var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            // Act
            var response = await _client.DeleteAsync($"/api/customers/{createdCustomer.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the deletion
            var getResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
} 
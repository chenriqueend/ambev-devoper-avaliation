using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.WebApi.Models.Branches;
using Ambev.DeveloperEvaluation.WebApi.Tests.Integration;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Integration.Branches
{
    public class BranchesControllerTests : TestBase
    {
        private readonly HttpClient _client;

        public BranchesControllerTests()
        {
            _client = CreateClient();
        }

        [Fact]
        public async Task CreateBranch_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var request = new BranchRequest
            {
                Name = "Test Branch",
                Address = "123 Test Street",
                Phone = "1234567890"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/branches", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var branch = await response.Content.ReadFromJsonAsync<BranchResponse>();
            Assert.NotNull(branch);
            Assert.Equal(request.Name, branch.Name);
            Assert.Equal(request.Address, branch.Address);
            Assert.Equal(request.Phone, branch.Phone);
        }

        [Fact]
        public async Task CreateBranch_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new BranchRequest
            {
                Name = "", // Invalid name
                Address = "123 Test Street",
                Phone = "1234567890"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/branches", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetBranch_WithValidId_ShouldReturnBranch()
        {
            // Arrange
            var createRequest = new BranchRequest
            {
                Name = "Test Branch",
                Address = "123 Test Street",
                Phone = "1234567890"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/branches", createRequest);
            var createdBranch = await createResponse.Content.ReadFromJsonAsync<BranchResponse>();

            // Act
            var response = await _client.GetAsync($"/api/branches/{createdBranch.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var branch = await response.Content.ReadFromJsonAsync<BranchResponse>();
            Assert.NotNull(branch);
            Assert.Equal(createdBranch.Id, branch.Id);
            Assert.Equal(createRequest.Name, branch.Name);
        }

        [Fact]
        public async Task GetBranch_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync($"/api/branches/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBranch_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var createRequest = new BranchRequest
            {
                Name = "Test Branch",
                Address = "123 Test Street",
                Phone = "1234567890"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/branches", createRequest);
            var createdBranch = await createResponse.Content.ReadFromJsonAsync<BranchResponse>();

            var updateRequest = new BranchRequest
            {
                Name = "Updated Branch",
                Address = "456 Updated Street",
                Phone = "0987654321"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/branches/{createdBranch.Id}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/branches/{createdBranch.Id}");
            var updatedBranch = await getResponse.Content.ReadFromJsonAsync<BranchResponse>();
            Assert.Equal(updateRequest.Name, updatedBranch.Name);
            Assert.Equal(updateRequest.Address, updatedBranch.Address);
            Assert.Equal(updateRequest.Phone, updatedBranch.Phone);
        }

        [Fact]
        public async Task DeleteBranch_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var createRequest = new BranchRequest
            {
                Name = "Test Branch",
                Address = "123 Test Street",
                Phone = "1234567890"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/branches", createRequest);
            var createdBranch = await createResponse.Content.ReadFromJsonAsync<BranchResponse>();

            // Act
            var response = await _client.DeleteAsync($"/api/branches/{createdBranch.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the deletion
            var getResponse = await _client.GetAsync($"/api/branches/{createdBranch.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
} 
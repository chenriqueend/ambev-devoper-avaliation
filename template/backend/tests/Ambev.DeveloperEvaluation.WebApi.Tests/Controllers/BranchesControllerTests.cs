using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Application.Commands.Branches;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Controllers;
using Ambev.DeveloperEvaluation.WebApi.Models.Branches;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Controllers
{
    public class BranchesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IBranchRepository> _branchRepositoryMock;
        private readonly BranchesController _controller;

        public BranchesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _branchRepositoryMock = new Mock<IBranchRepository>();
            _controller = new BranchesController(_mediatorMock.Object, _branchRepositoryMock.Object);
        }

        [Fact]
        public async Task Create_WithValidRequest_ShouldReturnCreated()
        {
            // Arrange
            var request = new BranchRequest
            {
                Name = "Test Branch",
                Address = "Test Address",
                Phone = "1234567890"
            };

            var branchId = Guid.NewGuid();
            var branch = new Branch(request.Name, request.Address, request.Phone)
            {
                Id = branchId
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateBranchCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(branch);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(BranchesController.GetById), createdResult.ActionName);
            Assert.Equal(branchId, createdResult.RouteValues["id"]);
            var response = Assert.IsType<BranchResponse>(createdResult.Value);
            Assert.Equal(branchId, response.Id);
            Assert.Equal(request.Name, response.Name);
            Assert.Equal(request.Address, response.Address);
            Assert.Equal(request.Phone, response.Phone);
        }

        [Fact]
        public async Task GetById_WithExistingBranch_ShouldReturnOk()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var branch = new Branch("Test Branch", "Test Address", "1234567890")
            {
                Id = branchId
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBranchByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(branch);

            // Act
            var result = await _controller.GetById(branchId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<BranchResponse>(okResult.Value);
            Assert.Equal(branchId, response.Id);
            Assert.Equal(branch.Name, response.Name);
            Assert.Equal(branch.Address, response.Address);
            Assert.Equal(branch.Phone, response.Phone);
        }

        [Fact]
        public async Task GetById_WithNonExistingBranch_ShouldReturnNotFound()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBranchByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Branch)null);

            // Act
            var result = await _controller.GetById(branchId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_WithValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var request = new BranchRequest
            {
                Name = "Updated Branch",
                Address = "Updated Address",
                Phone = "0987654321"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBranchCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(branchId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_WithNonExistingBranch_ShouldReturnNotFound()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var request = new BranchRequest
            {
                Name = "Updated Branch",
                Address = "Updated Address",
                Phone = "0987654321"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBranchCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(branchId, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WithExistingBranch_ShouldReturnNoContent()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBranchCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(branchId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WithNonExistingBranch_ShouldReturnNotFound()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBranchCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(branchId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllBranches()
        {
            // Arrange
            var branches = new List<Branch>
            {
                new Branch("Branch 1", "Address 1", "1234567890") { Id = Guid.NewGuid() },
                new Branch("Branch 2", "Address 2", "0987654321") { Id = Guid.NewGuid() }
            };

            _branchRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(branches);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<BranchResponse>>(okResult.Value);
            var branchList = response.ToList();
            Assert.Equal(2, branchList.Count);
            Assert.Equal(branches[0].Name, branchList[0].Name);
            Assert.Equal(branches[1].Name, branchList[1].Name);
        }

        [Fact]
        public async Task GetAll_WhenRepositoryThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            _branchRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Database error", badRequestResult.Value.ToString());
        }
    }
} 
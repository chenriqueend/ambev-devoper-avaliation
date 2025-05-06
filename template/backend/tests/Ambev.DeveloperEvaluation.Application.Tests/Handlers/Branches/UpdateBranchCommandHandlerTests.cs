using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Commands.Branches;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Application.Tests.Handlers.Branches
{
    public class UpdateBranchCommandHandlerTests
    {
        private readonly Mock<IBranchRepository> _branchRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateBranchCommandHandler _handler;

        public UpdateBranchCommandHandlerTests()
        {
            _branchRepositoryMock = new Mock<IBranchRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateBranchCommandHandler(_branchRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldUpdateBranchSuccessfully()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var existingBranch = new Branch("Old Name", "Old Address", "1234567890")
            {
                Id = branchId
            };

            var command = new UpdateBranchCommand
            {
                Id = branchId,
                Name = "New Name",
                Address = "New Address",
                Phone = "0987654321"
            };

            _branchRepositoryMock.Setup(r => r.GetByIdAsync(branchId))
                .ReturnsAsync(existingBranch);

            _branchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Branch>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            _branchRepositoryMock.Verify(r => r.GetByIdAsync(branchId), Times.Once);
            _branchRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Branch>(b => 
                b.Id == branchId && 
                b.Name == command.Name && 
                b.Address == command.Address && 
                b.Phone == command.Phone)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingBranch_ShouldReturnFailure()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var command = new UpdateBranchCommand
            {
                Id = branchId,
                Name = "New Name",
                Address = "New Address",
                Phone = "0987654321"
            };

            _branchRepositoryMock.Setup(r => r.GetByIdAsync(branchId))
                .ReturnsAsync((Branch)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _branchRepositoryMock.Verify(r => r.GetByIdAsync(branchId), Times.Once);
            _branchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Branch>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldReturnFailure()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var command = new UpdateBranchCommand
            {
                Id = branchId,
                Name = string.Empty,
                Address = "New Address",
                Phone = "0987654321"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _branchRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _branchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Branch>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var existingBranch = new Branch("Old Name", "Old Address", "1234567890")
            {
                Id = branchId
            };

            var command = new UpdateBranchCommand
            {
                Id = branchId,
                Name = "New Name",
                Address = "New Address",
                Phone = "0987654321"
            };

            _branchRepositoryMock.Setup(r => r.GetByIdAsync(branchId))
                .ReturnsAsync(existingBranch);

            _branchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Branch>()))
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
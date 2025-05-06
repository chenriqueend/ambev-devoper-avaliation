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
    public class DeleteBranchCommandHandlerTests
    {
        private readonly Mock<IBranchRepository> _branchRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteBranchCommandHandler _handler;

        public DeleteBranchCommandHandlerTests()
        {
            _branchRepositoryMock = new Mock<IBranchRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteBranchCommandHandler(_branchRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldDeleteBranchSuccessfully()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var existingBranch = new Branch("Test Branch", "Test Address", "1234567890")
            {
                Id = branchId
            };

            var command = new DeleteBranchCommand
            {
                Id = branchId
            };

            _branchRepositoryMock.Setup(r => r.GetByIdAsync(branchId))
                .ReturnsAsync(existingBranch);

            _branchRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Branch>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            _branchRepositoryMock.Verify(r => r.GetByIdAsync(branchId), Times.Once);
            _branchRepositoryMock.Verify(r => r.DeleteAsync(It.Is<Branch>(b => b.Id == branchId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingBranch_ShouldReturnFailure()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var command = new DeleteBranchCommand
            {
                Id = branchId
            };

            _branchRepositoryMock.Setup(r => r.GetByIdAsync(branchId))
                .ReturnsAsync((Branch)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _branchRepositoryMock.Verify(r => r.GetByIdAsync(branchId), Times.Once);
            _branchRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Branch>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var branchId = Guid.NewGuid();
            var existingBranch = new Branch("Test Branch", "Test Address", "1234567890")
            {
                Id = branchId
            };

            var command = new DeleteBranchCommand
            {
                Id = branchId
            };

            _branchRepositoryMock.Setup(r => r.GetByIdAsync(branchId))
                .ReturnsAsync(existingBranch);

            _branchRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Branch>()))
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
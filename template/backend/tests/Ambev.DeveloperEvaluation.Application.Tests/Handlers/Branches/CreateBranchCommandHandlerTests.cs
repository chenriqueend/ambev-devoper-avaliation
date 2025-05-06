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
    public class CreateBranchCommandHandlerTests
    {
        private readonly Mock<IBranchRepository> _branchRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateBranchCommandHandler _handler;

        public CreateBranchCommandHandlerTests()
        {
            _branchRepositoryMock = new Mock<IBranchRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateBranchCommandHandler(_branchRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateBranchSuccessfully()
        {
            // Arrange
            var command = new CreateBranchCommand
            {
                Name = "Test Branch",
                Address = "Test Address",
                Phone = "1234567890"
            };

            _branchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Branch>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(command.Name);
            result.Value.Address.Should().Be(command.Address);
            result.Value.Phone.Should().Be(command.Phone);

            _branchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Branch>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateBranchCommand
            {
                Name = string.Empty,
                Address = "Test Address",
                Phone = "1234567890"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNullOrEmpty();

            _branchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Branch>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateBranchCommand
            {
                Name = "Test Branch",
                Address = "Test Address",
                Phone = "1234567890"
            };

            _branchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Branch>()))
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
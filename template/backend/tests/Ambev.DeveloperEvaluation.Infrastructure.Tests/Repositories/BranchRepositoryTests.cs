using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Infrastructure.Data;
using Ambev.DeveloperEvaluation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Infrastructure.Tests.Repositories
{
    public class BranchRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly BranchRepository _repository;

        public BranchRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new BranchRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingBranch_ShouldReturnBranch()
        {
            // Arrange
            var branch = new Branch("Test Branch", "Test Address", "1234567890");
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(branch.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(branch.Id, result.Id);
            Assert.Equal(branch.Name, result.Name);
            Assert.Equal(branch.Address, result.Address);
            Assert.Equal(branch.Phone, result.Phone);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingBranch_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddBranchToDatabase()
        {
            // Arrange
            var branch = new Branch("Test Branch", "Test Address", "1234567890");

            // Act
            await _repository.AddAsync(branch);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Branches.FindAsync(branch.Id);
            Assert.NotNull(result);
            Assert.Equal(branch.Id, result.Id);
            Assert.Equal(branch.Name, result.Name);
            Assert.Equal(branch.Address, result.Address);
            Assert.Equal(branch.Phone, result.Phone);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingBranch_ShouldUpdateBranch()
        {
            // Arrange
            var branch = new Branch("Test Branch", "Test Address", "1234567890");
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();

            var updatedBranch = new Branch("Updated Branch", "Updated Address", "0987654321")
            {
                Id = branch.Id
            };

            // Act
            await _repository.UpdateAsync(updatedBranch);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Branches.FindAsync(branch.Id);
            Assert.NotNull(result);
            Assert.Equal(updatedBranch.Name, result.Name);
            Assert.Equal(updatedBranch.Address, result.Address);
            Assert.Equal(updatedBranch.Phone, result.Phone);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingBranch_ShouldThrowException()
        {
            // Arrange
            var branch = new Branch("Test Branch", "Test Address", "1234567890")
            {
                Id = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.UpdateAsync(branch));
        }

        [Fact]
        public async Task DeleteAsync_WithExistingBranch_ShouldRemoveBranch()
        {
            // Arrange
            var branch = new Branch("Test Branch", "Test Address", "1234567890");
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(branch);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Branches.FindAsync(branch.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingBranch_ShouldThrowException()
        {
            // Arrange
            var branch = new Branch("Test Branch", "Test Address", "1234567890")
            {
                Id = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.DeleteAsync(branch));
        }
    }
} 
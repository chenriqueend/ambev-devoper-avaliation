using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Infrastructure.Data;
using Ambev.DeveloperEvaluation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Infrastructure.Tests.Repositories
{
    public class CustomerRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomerRepository _repository;

        public CustomerRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new CustomerRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingCustomer_ShouldReturnCustomer()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@email.com", "1234567890");
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(customer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
            Assert.Equal(customer.Name, result.Name);
            Assert.Equal(customer.Email, result.Email);
            Assert.Equal(customer.Phone, result.Phone);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingCustomer_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddCustomerToDatabase()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@email.com", "1234567890");

            // Act
            await _repository.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Customers.FindAsync(customer.Id);
            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
            Assert.Equal(customer.Name, result.Name);
            Assert.Equal(customer.Email, result.Email);
            Assert.Equal(customer.Phone, result.Phone);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingCustomer_ShouldUpdateCustomer()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@email.com", "1234567890");
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var updatedCustomer = new Customer("Updated Customer", "updated@email.com", "0987654321")
            {
                Id = customer.Id
            };

            // Act
            await _repository.UpdateAsync(updatedCustomer);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Customers.FindAsync(customer.Id);
            Assert.NotNull(result);
            Assert.Equal(updatedCustomer.Name, result.Name);
            Assert.Equal(updatedCustomer.Email, result.Email);
            Assert.Equal(updatedCustomer.Phone, result.Phone);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingCustomer_ShouldThrowException()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@email.com", "1234567890")
            {
                Id = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.UpdateAsync(customer));
        }

        [Fact]
        public async Task DeleteAsync_WithExistingCustomer_ShouldRemoveCustomer()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@email.com", "1234567890");
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(customer);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Customers.FindAsync(customer.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingCustomer_ShouldThrowException()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@email.com", "1234567890")
            {
                Id = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.DeleteAsync(customer));
        }
    }
} 
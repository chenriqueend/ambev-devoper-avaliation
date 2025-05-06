using System;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Domain.Tests
{
    public class CustomerTests
    {
        [Fact]
        public void CreateCustomer_WithValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var name = "Test Customer";
            var email = "test@example.com";
            var phone = "1234567890";

            // Act
            var customer = new Customer(name, email, phone);

            // Assert
            customer.Name.Should().Be(name);
            customer.Email.Should().Be(email);
            customer.Phone.Should().Be(phone);
        }

        [Fact]
        public void CreateCustomer_WithEmptyName_ShouldThrowException()
        {
            // Arrange
            var name = string.Empty;
            var email = "test@example.com";
            var phone = "1234567890";

            // Act & Assert
            var action = () => new Customer(name, email, phone);
            action.Should().Throw<DomainException>().WithMessage("Customer name cannot be empty");
        }

        [Fact]
        public void CreateCustomer_WithEmptyEmail_ShouldThrowException()
        {
            // Arrange
            var name = "Test Customer";
            var email = string.Empty;
            var phone = "1234567890";

            // Act & Assert
            var action = () => new Customer(name, email, phone);
            action.Should().Throw<DomainException>().WithMessage("Email cannot be empty");
        }

        [Fact]
        public void CreateCustomer_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var name = "Test Customer";
            var email = "invalid-email";
            var phone = "1234567890";

            // Act & Assert
            var action = () => new Customer(name, email, phone);
            action.Should().Throw<DomainException>().WithMessage("Invalid email format");
        }

        [Fact]
        public void CreateCustomer_WithEmptyPhone_ShouldThrowException()
        {
            // Arrange
            var name = "Test Customer";
            var email = "test@example.com";
            var phone = string.Empty;

            // Act & Assert
            var action = () => new Customer(name, email, phone);
            action.Should().Throw<DomainException>().WithMessage("Phone cannot be empty");
        }

        [Fact]
        public void UpdateCustomer_WithValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@example.com", "1234567890");
            var newName = "Updated Customer";
            var newEmail = "updated@example.com";
            var newPhone = "0987654321";

            // Act
            customer.Update(newName, newEmail, newPhone);

            // Assert
            customer.Name.Should().Be(newName);
            customer.Email.Should().Be(newEmail);
            customer.Phone.Should().Be(newPhone);
        }

        [Fact]
        public void UpdateCustomer_WithEmptyName_ShouldThrowException()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@example.com", "1234567890");
            var newName = string.Empty;
            var newEmail = "updated@example.com";
            var newPhone = "0987654321";

            // Act & Assert
            var action = () => customer.Update(newName, newEmail, newPhone);
            action.Should().Throw<DomainException>().WithMessage("Customer name cannot be empty");
        }

        [Fact]
        public void UpdateCustomer_WithEmptyEmail_ShouldThrowException()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@example.com", "1234567890");
            var newName = "Updated Customer";
            var newEmail = string.Empty;
            var newPhone = "0987654321";

            // Act & Assert
            var action = () => customer.Update(newName, newEmail, newPhone);
            action.Should().Throw<DomainException>().WithMessage("Email cannot be empty");
        }

        [Fact]
        public void UpdateCustomer_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@example.com", "1234567890");
            var newName = "Updated Customer";
            var newEmail = "invalid-email";
            var newPhone = "0987654321";

            // Act & Assert
            var action = () => customer.Update(newName, newEmail, newPhone);
            action.Should().Throw<DomainException>().WithMessage("Invalid email format");
        }

        [Fact]
        public void UpdateCustomer_WithEmptyPhone_ShouldThrowException()
        {
            // Arrange
            var customer = new Customer("Test Customer", "test@example.com", "1234567890");
            var newName = "Updated Customer";
            var newEmail = "updated@example.com";
            var newPhone = string.Empty;

            // Act & Assert
            var action = () => customer.Update(newName, newEmail, newPhone);
            action.Should().Throw<DomainException>().WithMessage("Phone cannot be empty");
        }
    }
}
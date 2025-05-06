using System;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Domain.Tests
{
    public class BranchTests
    {
        [Fact]
        public void CreateBranch_WithValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var name = "Test Branch";
            var address = "123 Test Street";
            var phone = "1234567890";

            // Act
            var branch = new Branch(name, address, phone);

            // Assert
            branch.Name.Should().Be(name);
            branch.Address.Should().Be(address);
            branch.Phone.Should().Be(phone);
        }

        [Fact]
        public void CreateBranch_WithEmptyName_ShouldThrowException()
        {
            // Arrange
            var name = string.Empty;
            var address = "123 Test Street";
            var phone = "1234567890";

            // Act & Assert
            var action = () => new Branch(name, address, phone);
            action.Should().Throw<DomainException>().WithMessage("Branch name cannot be empty");
        }

        [Fact]
        public void CreateBranch_WithEmptyAddress_ShouldThrowException()
        {
            // Arrange
            var name = "Test Branch";
            var address = string.Empty;
            var phone = "1234567890";

            // Act & Assert
            var action = () => new Branch(name, address, phone);
            action.Should().Throw<DomainException>().WithMessage("Address cannot be empty");
        }

        [Fact]
        public void CreateBranch_WithEmptyPhone_ShouldThrowException()
        {
            // Arrange
            var name = "Test Branch";
            var address = "123 Test Street";
            var phone = string.Empty;

            // Act & Assert
            var action = () => new Branch(name, address, phone);
            action.Should().Throw<DomainException>().WithMessage("Phone cannot be empty");
        }

        [Fact]
        public void UpdateBranch_WithValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var branch = new Branch("Test Branch", "123 Test Street", "1234567890");
            var newName = "Updated Branch";
            var newAddress = "456 New Street";
            var newPhone = "0987654321";

            // Act
            branch.Update(newName, newAddress, newPhone);

            // Assert
            branch.Name.Should().Be(newName);
            branch.Address.Should().Be(newAddress);
            branch.Phone.Should().Be(newPhone);
        }

        [Fact]
        public void UpdateBranch_WithEmptyName_ShouldThrowException()
        {
            // Arrange
            var branch = new Branch("Test Branch", "123 Test Street", "1234567890");
            var newName = string.Empty;
            var newAddress = "456 New Street";
            var newPhone = "0987654321";

            // Act & Assert
            var action = () => branch.Update(newName, newAddress, newPhone);
            action.Should().Throw<DomainException>().WithMessage("Branch name cannot be empty");
        }

        [Fact]
        public void UpdateBranch_WithEmptyAddress_ShouldThrowException()
        {
            // Arrange
            var branch = new Branch("Test Branch", "123 Test Street", "1234567890");
            var newName = "Updated Branch";
            var newAddress = string.Empty;
            var newPhone = "0987654321";

            // Act & Assert
            var action = () => branch.Update(newName, newAddress, newPhone);
            action.Should().Throw<DomainException>().WithMessage("Address cannot be empty");
        }

        [Fact]
        public void UpdateBranch_WithEmptyPhone_ShouldThrowException()
        {
            // Arrange
            var branch = new Branch("Test Branch", "123 Test Street", "1234567890");
            var newName = "Updated Branch";
            var newAddress = "456 New Street";
            var newPhone = string.Empty;

            // Act & Assert
            var action = () => branch.Update(newName, newAddress, newPhone);
            action.Should().Throw<DomainException>().WithMessage("Phone cannot be empty");
        }
    }
}
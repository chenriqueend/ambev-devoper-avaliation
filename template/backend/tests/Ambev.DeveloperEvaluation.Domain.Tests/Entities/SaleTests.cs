using System;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Domain.Tests
{
    public class SaleTests
    {
        [Fact]
        public void CreateSale_WithValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var sale = new Sale(
                "SALE001",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch",
                0m
            );

            // Act
            var item1 = new SaleItem(Guid.NewGuid(), "Product 1", 5, 10.0m, sale);
            var item2 = new SaleItem(Guid.NewGuid(), "Product 2", 5, 15.0m, sale);
            sale.AddItem(item1);
            sale.AddItem(item2);

            // Assert - Total quantity is 10, should get item-level discounts only
            // Item 1: 5 units at $10 = $50 (10% item discount = $45)
            // Item 2: 5 units at $15 = $75 (10% item discount = $67.5)
            // Total after item discounts: $112.50
            sale.TotalWithDiscount.Should().Be(112.50m);
        }

        [Fact]
        public void CreateSale_WithEmptyItems_ShouldCreateSuccessfully()
        {
            // Arrange
            var saleNumber = "SALE001";
            var saleDate = DateTime.Now;
            var customerId = Guid.NewGuid();
            var customerName = "Test Customer";
            var branchId = Guid.NewGuid();
            var branchName = "Test Branch";
            var totalWithDiscount = 0m;

            // Act
            var sale = new Sale(saleNumber, saleDate, customerId, customerName, branchId, branchName, totalWithDiscount);

            // Assert
            sale.Items.Should().BeEmpty();
            sale.TotalWithDiscount.Should().Be(0);
        }

        [Fact]
        public void CreateSale_WithEmptySaleNumber_ShouldThrowException()
        {
            // Arrange
            var saleNumber = string.Empty;
            var saleDate = DateTime.Now;
            var customerId = Guid.NewGuid();
            var customerName = "Test Customer";
            var branchId = Guid.NewGuid();
            var branchName = "Test Branch";
            var totalWithDiscount = 0m;

            // Act & Assert
            var action = () => new Sale(saleNumber, saleDate, customerId, customerName, branchId, branchName, totalWithDiscount);
            action.Should().Throw<DomainException>().WithMessage("Sale number cannot be empty");
        }

        [Fact]
        public void CreateSale_WithEmptyCustomerName_ShouldThrowException()
        {
            // Arrange
            var saleNumber = "SALE001";
            var saleDate = DateTime.Now;
            var customerId = Guid.NewGuid();
            var customerName = string.Empty;
            var branchId = Guid.NewGuid();
            var branchName = "Test Branch";
            var totalWithDiscount = 0m;

            // Act & Assert
            var action = () => new Sale(saleNumber, saleDate, customerId, customerName, branchId, branchName, totalWithDiscount);
            action.Should().Throw<DomainException>().WithMessage("Customer name cannot be empty");
        }

        [Fact]
        public void CreateSale_WithEmptyBranchName_ShouldThrowException()
        {
            // Arrange
            var saleNumber = "SALE001";
            var saleDate = DateTime.Now;
            var customerId = Guid.NewGuid();
            var customerName = "Test Customer";
            var branchId = Guid.NewGuid();
            var branchName = string.Empty;
            var totalWithDiscount = 0m;

            // Act & Assert
            var action = () => new Sale(saleNumber, saleDate, customerId, customerName, branchId, branchName, totalWithDiscount);
            action.Should().Throw<DomainException>().WithMessage("Branch name cannot be empty");
        }

        [Fact]
        public void AddItems_WithQuantityDiscount_ShouldCalculateCorrectly()
        {
            // Arrange
            var sale = new Sale(
                "SALE001",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch",
                0m
            );

            // Act
            var item1 = new SaleItem(Guid.NewGuid(), "Product 1", 5, 10.0m, sale);
            var item2 = new SaleItem(Guid.NewGuid(), "Product 2", 5, 15.0m, sale);
            sale.AddItem(item1);
            sale.AddItem(item2);

            // Assert - Total quantity is 10, should get item-level discounts only
            // Item 1: 5 units at $10 = $50 (10% item discount = $45)
            // Item 2: 5 units at $15 = $75 (10% item discount = $67.5)
            // Total after item discounts: $112.50
            sale.TotalWithDiscount.Should().Be(112.50m);
        }

        [Fact]
        public void AddItems_WithLargeQuantityDiscount_ShouldCalculateCorrectly()
        {
            // Arrange
            var sale = new Sale(
                "SALE001",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch",
                0m
            );

            // Act
            var item1 = new SaleItem(Guid.NewGuid(), "Product 1", 5, 10.0m, sale);
            var item2 = new SaleItem(Guid.NewGuid(), "Product 2", 5, 15.0m, sale);
            sale.AddItem(item1);
            sale.AddItem(item2);

            // Assert - Total quantity is 10, should get item-level discounts only
            // Item 1: 5 units at $10 = $50 (10% item discount = $45)
            // Item 2: 5 units at $15 = $75 (10% item discount = $67.5)
            // Total after item discounts: $112.50
            sale.TotalWithDiscount.Should().Be(112.50m);
        }

        [Fact]
        public void CancelSale_ShouldMarkSaleAsCancelled()
        {
            // Arrange
            var sale = new Sale(
                "SALE001",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch",
                0m
            );
            var item = new SaleItem(Guid.NewGuid(), "Product 1", 1, 10.0m, sale);
            sale.AddItem(item);

            // Act
            sale.Cancel();

            // Assert
            sale.IsCancelled.Should().BeTrue();
        }

        [Fact]
        public void CancelSale_WhenAlreadyCancelled_ShouldThrowException()
        {
            // Arrange
            var sale = new Sale(
                "SALE001",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch",
                0m
            );
            var item = new SaleItem(Guid.NewGuid(), "Product 1", 1, 10.0m, sale);
            sale.AddItem(item);
            sale.Cancel();

            // Act & Assert
            var action = () => sale.Cancel();
            action.Should().Throw<DomainException>().WithMessage("Sale is already cancelled.");
        }
    }
}
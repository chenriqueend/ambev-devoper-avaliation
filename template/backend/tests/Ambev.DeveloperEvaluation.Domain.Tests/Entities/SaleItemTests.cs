using System;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Domain.Tests
{
    public class SaleItemTests
    {
        private Sale CreateSale()
        {
            return new Sale(
                "SALE001",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch",
                0m
            );
        }

        [Fact]
        public void CreateSaleItem_WithValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDescription = "Test Product";
            var quantity = 2;
            var unitPrice = 10.0m;
            var sale = CreateSale();

            // Act
            var item = new SaleItem(productId, productDescription, quantity, unitPrice, sale);

            // Assert
            item.ProductId.Should().Be(productId);
            item.ProductDescription.Should().Be(productDescription);
            item.Quantity.Should().Be(quantity);
            item.UnitPrice.Should().Be(unitPrice);
            item.TotalAmount.Should().Be(20.0m); // 2 * 10.0
            item.Discount.Should().Be(0.0m); // No discount for quantity < 4
            item.Sale.Should().Be(sale);
        }

        [Fact]
        public void CreateSaleItem_WithEmptyDescription_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDescription = string.Empty;
            var quantity = 1;
            var unitPrice = 10.0m;
            var sale = CreateSale();

            // Act & Assert
            var action = () => new SaleItem(productId, productDescription, quantity, unitPrice, sale);
            action.Should().Throw<DomainException>().WithMessage("Product description cannot be empty");
        }

        [Fact]
        public void CreateSaleItem_WithZeroQuantity_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDescription = "Test Product";
            var quantity = 0;
            var unitPrice = 10.0m;
            var sale = CreateSale();

            // Act & Assert
            var action = () => new SaleItem(productId, productDescription, quantity, unitPrice, sale);
            action.Should().Throw<DomainException>().WithMessage("Quantity must be greater than 0");
        }

        [Fact]
        public void CreateSaleItem_WithNegativeQuantity_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDescription = "Test Product";
            var quantity = -1;
            var unitPrice = 10.0m;
            var sale = CreateSale();

            // Act & Assert
            var action = () => new SaleItem(productId, productDescription, quantity, unitPrice, sale);
            action.Should().Throw<DomainException>().WithMessage("Quantity must be greater than 0");
        }

        [Fact]
        public void CreateSaleItem_WithZeroUnitPrice_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDescription = "Test Product";
            var quantity = 1;
            var unitPrice = 0.0m;
            var sale = CreateSale();

            // Act & Assert
            var action = () => new SaleItem(productId, productDescription, quantity, unitPrice, sale);
            action.Should().Throw<DomainException>().WithMessage("Unit price must be greater than 0");
        }

        [Fact]
        public void CreateSaleItem_WithNegativeUnitPrice_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDescription = "Test Product";
            var quantity = 1;
            var unitPrice = -10.0m;
            var sale = CreateSale();

            // Act & Assert
            var action = () => new SaleItem(productId, productDescription, quantity, unitPrice, sale);
            action.Should().Throw<DomainException>().WithMessage("Unit price must be greater than 0");
        }

        [Fact]
        public void UpdateQuantity_WithValidQuantity_ShouldUpdateSuccessfully()
        {
            // Arrange
            var sale = CreateSale();
            var item = new SaleItem(Guid.NewGuid(), "Test Product", 1, 10.0m, sale);
            var newQuantity = 3;

            // Act
            item.UpdateQuantity(newQuantity);

            // Assert
            item.Quantity.Should().Be(newQuantity);
            item.TotalAmount.Should().Be(30.0m); // 3 * 10.0
        }

        [Fact]
        public void UpdateQuantity_WithZeroQuantity_ShouldThrowException()
        {
            // Arrange
            var sale = CreateSale();
            var item = new SaleItem(Guid.NewGuid(), "Test Product", 1, 10.0m, sale);
            var newQuantity = 0;

            // Act & Assert
            var action = () => item.UpdateQuantity(newQuantity);
            action.Should().Throw<DomainException>().WithMessage("Quantity must be greater than 0");
        }

        [Fact]
        public void UpdateQuantity_WithNegativeQuantity_ShouldThrowException()
        {
            // Arrange
            var sale = CreateSale();
            var item = new SaleItem(Guid.NewGuid(), "Test Product", 1, 10.0m, sale);
            var newQuantity = -1;

            // Act & Assert
            var action = () => item.UpdateQuantity(newQuantity);
            action.Should().Throw<DomainException>().WithMessage("Quantity must be greater than 0");
        }

        [Fact]
        public void Cancel_ShouldMarkItemAsCancelled()
        {
            // Arrange
            var sale = CreateSale();
            var item = new SaleItem(Guid.NewGuid(), "Test Product", 1, 10.0m, sale);

            // Act
            item.Cancel();

            // Assert
            item.IsCancelled.Should().BeTrue();
        }

        [Fact]
        public void Cancel_WhenAlreadyCancelled_ShouldThrowException()
        {
            // Arrange
            var sale = CreateSale();
            var item = new SaleItem(Guid.NewGuid(), "Test Product", 1, 10.0m, sale);
            item.Cancel();

            // Act & Assert
            var action = () => item.Cancel();
            action.Should().Throw<DomainException>().WithMessage("Item is already cancelled");
        }
    }
}
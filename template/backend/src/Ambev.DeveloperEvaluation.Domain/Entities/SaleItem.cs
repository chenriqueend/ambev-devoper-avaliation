using System;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents an item in a sale, containing product information and quantity calculations.
    /// </summary>
    public class SaleItem : Entity
    {
        // External Identity for Product
        public Guid ProductId { get; private set; }
        public string ProductDescription { get; private set; } = string.Empty;

        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsCancelled { get; private set; }

        // Navigation property
        public Guid SaleId { get; private set; }
        public Sale Sale { get; private set; } = null!;

        protected SaleItem() { }

        public SaleItem(Guid productId, string productDescription, int quantity, decimal unitPrice, Sale sale)
        {
            if (string.IsNullOrEmpty(productDescription))
                throw new DomainException("Product description cannot be empty");

            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than 0");

            if (unitPrice <= 0)
                throw new DomainException("Unit price must be greater than 0");

            ProductId = productId;
            ProductDescription = productDescription;
            Quantity = quantity;
            UnitPrice = unitPrice;
            IsCancelled = false;
            Sale = sale;
            SaleId = sale.Id;

            CalculateTotalAmount();
        }

        /// <summary>
        /// Updates the quantity of the item, recalculating total amount.
        /// </summary>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new DomainException("Quantity must be greater than 0");

            if (newQuantity > 20)
                throw new DomainException("Cannot sell more than 20 identical items.");

            Quantity = newQuantity;
            CalculateTotalAmount();
        }

        private void CalculateTotalAmount()
        {
            var rawTotal = Quantity * UnitPrice;

            // Calculate item-level discount based on quantity
            // 4+ items: 10% discount
            // 10-20 items: 20% discount
            decimal itemDiscountPercentage = Quantity >= 10 ? 0.20m : Quantity >= 4 ? 0.10m : 0;
            Discount = rawTotal * itemDiscountPercentage;
            TotalAmount = rawTotal - Discount;
        }

        /// <summary>
        /// Marks the item as cancelled, preventing further modifications.
        /// </summary>
        public void Cancel()
        {
            if (IsCancelled)
                throw new DomainException("Item is already cancelled");

            IsCancelled = true;
        }
    }
}
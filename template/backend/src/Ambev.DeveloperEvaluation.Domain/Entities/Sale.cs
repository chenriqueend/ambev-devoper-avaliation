using System;
using System.Collections.Generic;
using System.Linq;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents a sale in the system, containing items and managing total calculations with discounts.
    /// </summary>
    public class Sale : AggregateRoot
    {
        public string SaleNumber { get; private set; } = string.Empty;
        public DateTime SaleDate { get; private set; }

        // External Identity for Customer
        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; } = string.Empty;

        // External Identity for Branch
        public Guid BranchId { get; private set; }
        public string BranchName { get; private set; } = string.Empty;

        public decimal TotalWithDiscount { get; private set; }
        public bool IsCancelled { get; private set; }

        private readonly List<SaleItem> _items;
        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

        protected Sale()
        {
            _items = new List<SaleItem>();
        }

        public Sale(string saleNumber, DateTime saleDate, Guid customerId, string customerName, Guid branchId, string branchName, decimal totalWithDiscount)
        {
            SaleNumber = saleNumber;
            SaleDate = saleDate;
            CustomerId = customerId;
            CustomerName = customerName;
            BranchId = branchId;
            BranchName = branchName;
            TotalWithDiscount = totalWithDiscount;
            IsCancelled = false;
            _items = new List<SaleItem>();

            Validate();

            AddDomainEvent(new StoredDomainEvent("SaleCreated",
                $"{{\"Id\":\"{Id}\",\"SaleNumber\":\"{SaleNumber}\",\"SaleDate\":\"{SaleDate}\",\"CustomerId\":\"{CustomerId}\",\"CustomerName\":\"{CustomerName}\",\"BranchId\":\"{BranchId}\",\"BranchName\":\"{BranchName}\",\"TotalWithDiscount\":{TotalWithDiscount}}}"));
        }

        /// <summary>
        /// Adds a new item to the sale, validating quantity limits and recalculating totals.
        /// </summary>
        /// <param name="item">The sale item to be added</param>
        /// <exception cref="DomainException">Thrown when sale is cancelled or quantity limit is exceeded</exception>
        public void AddItem(SaleItem item)
        {
            if (IsCancelled)
                throw new DomainException("Cannot add items to a cancelled sale.");

            var totalQuantity = _items.Where(i => !i.IsCancelled).Sum(i => i.Quantity) + item.Quantity;
            if (totalQuantity > 20)
                throw new DomainException("Total quantity across all items cannot exceed 20.");

            _items.Add(item);
            CalculateTotalAmount();

            AddDomainEvent(new StoredDomainEvent("ItemAdded",
                $"{{\"Id\":\"{Id}\",\"ProductId\":\"{item.ProductId}\",\"Quantity\":{item.Quantity},\"UnitPrice\":{item.UnitPrice},\"Discount\":{item.Discount},\"TotalAmount\":{item.TotalAmount}}}"));
            AddDomainEvent(new StoredDomainEvent("SaleModified",
                $"{{\"Id\":\"{Id}\",\"TotalWithDiscount\":{TotalWithDiscount}}}"));
        }

        /// <summary>
        /// Updates the quantity of an existing item in the sale, recalculating totals and discounts.
        /// </summary>
        /// <param name="productId">The ID of the product to update</param>
        /// <param name="newQuantity">The new quantity for the item</param>
        /// <exception cref="DomainException">Thrown when sale is cancelled, item not found, or quantity limit is exceeded</exception>
        public void UpdateItemQuantity(Guid productId, int newQuantity)
        {
            if (IsCancelled)
                throw new DomainException("Cannot update items in a cancelled sale.");

            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new DomainException("Item not found in sale.");

            var otherItemsQuantity = _items.Where(i => !i.IsCancelled && i.ProductId != productId).Sum(i => i.Quantity);
            if (otherItemsQuantity + newQuantity > 20)
                throw new DomainException("Total quantity across all items cannot exceed 20.");

            item.UpdateQuantity(newQuantity);
            CalculateTotalAmount();

            AddDomainEvent(new StoredDomainEvent("ItemQuantityUpdated",
                $"{{\"Id\":\"{Id}\",\"ProductId\":\"{productId}\",\"NewQuantity\":{newQuantity}}}"));
            AddDomainEvent(new StoredDomainEvent("SaleModified",
                $"{{\"Id\":\"{Id}\",\"TotalWithDiscount\":{TotalWithDiscount}}}"));
        }

        /// <summary>
        /// Removes an item from the sale and updates the total amount.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove</param>
        public void RemoveItem(Guid itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _items.Remove(item);
                CalculateTotalAmount();
                AddDomainEvent(new StoredDomainEvent("SaleModified",
                    $"{{\"Id\":\"{Id}\",\"TotalWithDiscount\":{TotalWithDiscount}}}"));
            }
        }

        /// <summary>
        /// Cancels a specific item in the sale, marking it as cancelled and recalculating totals.
        /// </summary>
        /// <param name="productId">The ID of the product to cancel</param>
        /// <exception cref="DomainException">Thrown when sale is cancelled or item not found</exception>
        public void CancelItem(Guid productId)
        {
            if (IsCancelled)
                throw new DomainException("Cannot cancel items in a cancelled sale.");

            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new DomainException("Item not found in sale.");

            item.Cancel();
            CalculateTotalAmount();

            AddDomainEvent(new StoredDomainEvent("ItemCancelled",
                $"{{\"Id\":\"{Id}\",\"ProductId\":\"{productId}\"}}"));
            AddDomainEvent(new StoredDomainEvent("SaleModified",
                $"{{\"Id\":\"{Id}\",\"TotalWithDiscount\":{TotalWithDiscount}}}"));
        }

        /// <summary>
        /// Cancels the entire sale, preventing further modifications.
        /// </summary>
        /// <exception cref="DomainException">Thrown when sale is already cancelled</exception>
        public void Cancel()
        {
            if (IsCancelled)
                throw new DomainException("Sale is already cancelled.");

            IsCancelled = true;
            AddDomainEvent(new SaleCancelledEvent(Id));
        }

        /// <summary>
        /// Calculates the total amount of the sale.
        /// The total is the sum of all items' TotalAmount (which already includes their discounts).
        /// </summary>
        private void CalculateTotalAmount()
        {
            var activeItems = _items.Where(i => !i.IsCancelled).ToList();
            var totalQuantity = activeItems.Sum(i => i.Quantity);
            var rawTotal = activeItems.Sum(i => i.TotalAmount);

            TotalWithDiscount = rawTotal;
        }

        private void Validate()
        {
            StringValidation.ValidateName(SaleNumber, "Sale number");
            StringValidation.ValidateName(CustomerName, "Customer name");
            StringValidation.ValidateName(BranchName, "Branch name");

            if (CustomerId == Guid.Empty)
                throw new DomainException("Customer ID cannot be empty.");

            if (BranchId == Guid.Empty)
                throw new DomainException("Branch ID cannot be empty.");
        }
    }
}
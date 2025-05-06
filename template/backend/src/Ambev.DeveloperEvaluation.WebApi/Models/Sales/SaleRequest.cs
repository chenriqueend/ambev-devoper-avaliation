using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ambev.DeveloperEvaluation.WebApi.Models.Sales
{
    /// <summary>
    /// Request model for creating a new sale
    /// </summary>
    public class SaleRequest
    {
        /// <summary>
        /// The unique number identifying the sale. Must not be empty and should follow a valid format (e.g., "SALE-001")
        /// </summary>
        [Required(ErrorMessage = "Sale number is required")]
        [StringLength(50, ErrorMessage = "Sale number must not exceed 50 characters")]
        public required string SaleNumber { get; set; }

        /// <summary>
        /// The unique identifier of the customer
        /// </summary>
        [Required(ErrorMessage = "Customer ID is required")]
        public required Guid CustomerId { get; set; }

        /// <summary>
        /// The name of the customer. Required for sale creation.
        /// </summary>
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(200, ErrorMessage = "Customer name must not exceed 200 characters")]
        public required string CustomerName { get; set; }

        /// <summary>
        /// The unique identifier of the branch
        /// </summary>
        [Required(ErrorMessage = "Branch ID is required")]
        public required Guid BranchId { get; set; }

        /// <summary>
        /// The name of the branch. Required for sale creation.
        /// </summary>
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(200, ErrorMessage = "Branch name must not exceed 200 characters")]
        public required string BranchName { get; set; }

        /// <summary>
        /// The list of items in the sale
        /// </summary>
        [Required(ErrorMessage = "At least one item is required")]
        public required List<SaleItemRequest> Items { get; set; }

        /// <summary>
        /// Request model for a sale item
        /// </summary>
        public class SaleItemRequest
        {
            /// <summary>
            /// The unique identifier of the product
            /// </summary>
            [Required(ErrorMessage = "Product ID is required")]
            public required Guid ProductId { get; set; }

            /// <summary>
            /// The description of the product
            /// </summary>
            [Required(ErrorMessage = "Product description is required")]
            [StringLength(200, ErrorMessage = "Product description must not exceed 200 characters")]
            public required string ProductDescription { get; set; }

            /// <summary>
            /// The quantity of the product. Must be greater than 0.
            /// Note: Discounts are automatically applied based on quantity:
            /// - 4-9 items: 10% discount
            /// - 10-20 items: 20% discount
            /// </summary>
            [Required(ErrorMessage = "Quantity is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
            [DefaultValue(20)]
            public int Quantity { get; set; }

            /// <summary>
            /// The unit price of the product. Must be greater than 0.
            /// </summary>
            [Required(ErrorMessage = "Unit price is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
            [DefaultValue(10.0)]
            public decimal UnitPrice { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Models.Sales
{
    public class SaleResponse
    {
        public Guid Id { get; set; }
        public required string SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public Guid CustomerId { get; set; }
        public required string CustomerName { get; set; }
        public Guid BranchId { get; set; }
        public required string BranchName { get; set; }
        public decimal TotalWithDiscount { get; set; }
        public bool IsCancelled { get; set; }
        public required List<SaleItemResponse> Items { get; set; }

        public class SaleItemResponse
        {
            public Guid ProductId { get; set; }
            public required string ProductDescription { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Discount { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}
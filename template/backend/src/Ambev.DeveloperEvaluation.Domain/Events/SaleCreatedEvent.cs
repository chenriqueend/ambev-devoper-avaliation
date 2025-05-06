using System;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleCreatedEvent : StoredDomainEvent
    {
        public SaleCreatedEvent(Guid saleId, string saleNumber, DateTime saleDate, Guid customerId, Guid branchId, decimal totalAmount)
            : base("SaleCreated", JsonSerializer.Serialize(new
            {
                SaleId = saleId,
                SaleNumber = saleNumber,
                SaleDate = saleDate,
                CustomerId = customerId,
                BranchId = branchId,
                TotalAmount = totalAmount
            }))
        {
        }
    }
}
using System;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleModifiedEvent : StoredDomainEvent
    {
        public SaleModifiedEvent(Guid saleId, decimal totalAmount)
            : base("SaleModified", JsonSerializer.Serialize(new
            {
                SaleId = saleId,
                TotalAmount = totalAmount
            }))
        {
        }
    }
}
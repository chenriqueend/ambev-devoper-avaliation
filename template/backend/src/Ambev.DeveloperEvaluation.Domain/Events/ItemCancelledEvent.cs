using System;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class ItemCancelledEvent : StoredDomainEvent
    {
        public ItemCancelledEvent(Guid saleId, Guid productId)
            : base("ItemCancelled", JsonSerializer.Serialize(new
            {
                SaleId = saleId,
                ProductId = productId
            }))
        {
        }
    }
}
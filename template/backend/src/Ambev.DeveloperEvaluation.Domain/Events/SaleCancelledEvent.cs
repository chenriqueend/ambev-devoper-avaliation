using System;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleCancelledEvent : StoredDomainEvent
    {
        public SaleCancelledEvent(Guid saleId)
            : base("SaleCancelled", JsonSerializer.Serialize(new
            {
                SaleId = saleId
            }))
        {
        }
    }
}
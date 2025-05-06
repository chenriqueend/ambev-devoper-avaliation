using System;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class StoredDomainEvent : Entity
    {
        public string EventType { get; private set; } = string.Empty;
        public string EventData { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }
        public Guid? SaleId { get; private set; }
        public Guid? BranchId { get; private set; }
        public Guid? CustomerId { get; private set; }

        protected StoredDomainEvent()
        {
        }

        public StoredDomainEvent(string eventType, string eventData, Guid? saleId = null, Guid? branchId = null, Guid? customerId = null)
        {
            EventType = eventType;
            EventData = eventData;
            Timestamp = DateTime.UtcNow;
            SaleId = saleId;
            BranchId = branchId;
            CustomerId = customerId;
        }
    }
}
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Common
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<StoredDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<StoredDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(StoredDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
using System;
using System.Collections.Generic;

namespace Booking.Domain.Abstractions;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; }
    
    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity(TId id)
    {
        Id = id;
    }

    protected void AddDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

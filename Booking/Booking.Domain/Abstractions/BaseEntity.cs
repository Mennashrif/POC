using System;
using System.Collections.Generic;

namespace Booking.Domain.Abstractions;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; }
    
    // Developer Note: Since we just talked about Domain Events, the BaseEntity 
    // is the perfect place to store them! An Entity creates events and holds them 
    // here until the database saves, then we publish them to RabbitMQ.
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

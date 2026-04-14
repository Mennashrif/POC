namespace Booking.Domain.Abstractions;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; }
    
    private readonly List<object> _domainEvents = new();

    protected BaseEntity(TId id)
    {
        Id = id;
    }

}

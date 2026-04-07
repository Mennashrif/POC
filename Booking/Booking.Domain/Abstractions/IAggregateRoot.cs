namespace Booking.Domain.Abstractions;

// Marker interface for DDD Aggregate Roots.
// Any Entity that implements this is considered the "Boss" of its object graph
// and is allowed to be saved/loaded directly from a Database Repository.
public interface IAggregateRoot
{
}

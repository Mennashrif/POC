namespace Booking.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}

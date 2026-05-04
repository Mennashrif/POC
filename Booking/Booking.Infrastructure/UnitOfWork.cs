using Booking.Application.Abstractions;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookingDbContext _dbContext;

    public UnitOfWork(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}

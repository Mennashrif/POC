using Booking.Application.Abstractions;
using Booking.Domain.Models;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class LocalRoomRepository : ILocalRoomRepository
{
    private readonly BookingDbContext _dbContext;

    public LocalRoomRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LocalRoom?> GetByIdAsync(Guid id)
    {
        return await _dbContext.LocalRooms.FindAsync(id);
    }

    public async Task<int> GetTotalCountByRoomTypeIdAsync(Guid roomTypeId)
    {
        return await _dbContext.LocalRooms
            .CountAsync(r => r.RoomTypeId == roomTypeId);
    }

    public async Task AddAsync(LocalRoom room)
    {
        await _dbContext.LocalRooms.AddAsync(room);
    }

    public void Update(LocalRoom room)
    {
        _dbContext.LocalRooms.Update(room);
    }
}

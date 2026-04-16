using Microsoft.EntityFrameworkCore;
using RoomManagement.Application.Abstractions;
using RoomManagement.Application.DTOs;
using RoomManagement.Domain.Models;
using RoomManagement.Infrastructure.Data;

namespace RoomManagement.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly RoomManagementDbContext _dbContext;

    public RoomRepository(RoomManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Rooms.FindAsync(id);
    }

    public async Task<Room?> GetByRoomNumberAsync(string roomNumber)
    {
        return await _dbContext.Rooms
            .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
    }

    public async Task<List<RoomDto>> GetAllAsync()
    {
        return await _dbContext.Rooms
            .AsNoTracking()
            .Include(r => r.RoomType)
            .Select(r => new RoomDto(
                r.Id,
                r.RoomNumber,
                r.RoomTypeId,
                r.RoomType!.Name,
                r.RoomType!.Price,
                r.RoomStatus.ToString()
            ))
            .ToListAsync();
    }

    public async Task AddAsync(Room room)
    {
        await _dbContext.Rooms.AddAsync(room);
    }

    public void Update(Room room)
    {
        _dbContext.Rooms.Update(room);
    }

    public void UpdateRange(List<Room> rooms)
    {
        _dbContext.Rooms.UpdateRange(rooms);
    }
    public Task<List<Room>> GetByRoomNumbersAsync(List<string> physicalRoomNumbers)
    {
        return _dbContext.Rooms
            .Where(r => physicalRoomNumbers.Contains(r.RoomNumber))
            .ToListAsync();
    }
}

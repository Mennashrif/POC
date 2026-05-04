using Microsoft.EntityFrameworkCore;
using RoomManagement.Application.Abstractions;
using RoomManagement.Application.DTOs;
using RoomManagement.Domain.Models;
using RoomManagement.Infrastructure.Data;

namespace RoomManagement.Infrastructure.Repositories;

public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly RoomManagementDbContext _dbContext;

    public RoomTypeRepository(RoomManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomType?> GetByIdAsync(Guid id)
    {
        return await _dbContext.RoomTypes.FindAsync(id);
    }

    public async Task<List<RoomTypeDto>> GetAllAsync()
    {
        return await _dbContext.RoomTypes
            .AsNoTracking()
            .Select(rt => new RoomTypeDto(rt.Id, rt.Name, rt.Price))
            .ToListAsync();
    }

    public async Task AddAsync(RoomType roomType)
    {
        await _dbContext.RoomTypes.AddAsync(roomType);
    }

}

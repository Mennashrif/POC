using Microsoft.EntityFrameworkCore;
using RoomManagement.Application.Abstractions;
using RoomManagement.Domain.Models;
using RoomManagement.Infrastructure.Data;

namespace RoomManagement.Infrastructure.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly RoomManagementDbContext _dbContext;

    public OutboxRepository(RoomManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage message)
    {
        await _dbContext.OutboxMessages.AddAsync(message);
    }

    public async Task<List<OutboxMessage>> GetUnpublishedAsync()
    {
        return await _dbContext.OutboxMessages
            .Where(m => m.PublishedAt == null)
            .OrderBy(m => m.OccurredAt)
            .ToListAsync();
    }

}

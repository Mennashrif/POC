using Microsoft.EntityFrameworkCore;
using RoomManagement.Application.Abstractions;
using RoomManagement.Domain.Models;
using RoomManagement.Infrastructure.Data;

namespace RoomManagement.Infrastructure.Repositories;

public class ProcessedEventRepository : IProcessedEventRepository
{
    private readonly RoomManagementDbContext _dbContext;

    public ProcessedEventRepository(RoomManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(string eventId, string eventType)
    {
        return await _dbContext.ProcessedEvents
            .AnyAsync(p => p.EventId == eventId && p.EventType == eventType);
    }

    public async Task AddAsync(ProcessedEvent processedEvent)
    {
        await _dbContext.ProcessedEvents.AddAsync(processedEvent);
    }

}

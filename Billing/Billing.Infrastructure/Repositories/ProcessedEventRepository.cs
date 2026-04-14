using Billing.Application.Abstractions;
using Billing.Domain.Models;
using Billing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure.Repositories;

public class ProcessedEventRepository : IProcessedEventRepository
{
    private readonly BillingDbContext _dbContext;

    public ProcessedEventRepository(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(Guid reservationId, string eventType)
    {
        return await _dbContext.ProcessedEvents
            .AnyAsync(p => p.ReservationId == reservationId && p.EventType == eventType);
    }

    public async Task AddAsync(ProcessedEvent processedEvent)
    {
        await _dbContext.ProcessedEvents.AddAsync(processedEvent);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}

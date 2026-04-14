namespace Billing.Application.Abstractions;

public interface IProcessedEventRepository
{
    Task<bool> ExistsAsync(Guid reservationId, string eventType);
    Task AddAsync(Domain.Models.ProcessedEvent processedEvent);
    Task SaveChangesAsync();
}

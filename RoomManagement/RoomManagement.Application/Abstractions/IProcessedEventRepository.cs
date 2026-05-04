using RoomManagement.Domain.Models;

namespace RoomManagement.Application.Abstractions;

public interface IProcessedEventRepository
{
    Task<bool> ExistsAsync(string eventId, string eventType);
    Task AddAsync(ProcessedEvent processedEvent);
}

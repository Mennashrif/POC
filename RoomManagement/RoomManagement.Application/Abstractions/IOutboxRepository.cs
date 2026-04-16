using RoomManagement.Domain.Models;

namespace RoomManagement.Application.Abstractions;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message);
    Task<List<OutboxMessage>> GetUnpublishedAsync();
}

using Booking.Domain.Models;

namespace Booking.Application.Abstractions;

public interface ILocalRoomRepository
{
    Task<LocalRoom?> GetByIdAsync(Guid id);
    Task<int> GetTotalCountByRoomTypeIdAsync(Guid roomTypeId);
    Task AddAsync(LocalRoom room);
    void Update(LocalRoom room);
}

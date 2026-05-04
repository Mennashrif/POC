using Booking.Domain.Abstractions;

namespace Booking.Application.Services;

public interface ILocalRoomService
{
    Task<Result<bool>> UpsertAsync(Guid roomId, string roomNumber, Guid roomTypeId, string status);
}

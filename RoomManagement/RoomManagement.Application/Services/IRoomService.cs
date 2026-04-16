using RoomManagement.Domain.Abstractions;
using RoomManagement.Domain.Models;

namespace RoomManagement.Application.Services;

public interface IRoomService
{
    Task<Result<Guid>> AddAsync(string roomNumber, Guid roomTypeId);
    Task<Result<bool>> UpdateAsync(Guid roomId, string roomNumber, Guid roomTypeId, RoomStatus status);
    Task CheckInAsync(Guid reservationId, List<string> physicalRoomNumbers);
}

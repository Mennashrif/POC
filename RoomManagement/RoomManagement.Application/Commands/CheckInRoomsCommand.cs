using MediatR;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public record CheckInRoomsCommand(
    Guid ReservationId,
    List<string> PhysicalRoomNumbers,
    DateTime OccurredAt
) : IRequest<Result<bool>>;

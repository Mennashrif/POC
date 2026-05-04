using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands;

public record UpsertLocalRoomCommand(
    Guid RoomId,
    string RoomNumber,
    Guid RoomTypeId,
    string Status
) : IRequest<Result<bool>>;

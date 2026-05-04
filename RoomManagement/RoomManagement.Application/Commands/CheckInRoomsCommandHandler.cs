using MediatR;
using RoomManagement.Application.Services;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public class CheckInRoomsCommandHandler : IRequestHandler<CheckInRoomsCommand, Result<bool>>
{
    private readonly IRoomService _roomService;

    public CheckInRoomsCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<Result<bool>> Handle(CheckInRoomsCommand command, CancellationToken cancellationToken)
    {
        await _roomService.CheckInAsync(command.ReservationId, command.PhysicalRoomNumbers);
        return Result<bool>.Success(true);
    }
}

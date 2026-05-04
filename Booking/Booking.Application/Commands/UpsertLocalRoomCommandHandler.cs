using Booking.Application.Services;
using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands;

public class UpsertLocalRoomCommandHandler : IRequestHandler<UpsertLocalRoomCommand, Result<bool>>
{
    private readonly ILocalRoomService _localRoomService;

    public UpsertLocalRoomCommandHandler(ILocalRoomService localRoomService)
    {
        _localRoomService = localRoomService;
    }

    public async Task<Result<bool>> Handle(UpsertLocalRoomCommand command, CancellationToken cancellationToken)
    {
        return await _localRoomService.UpsertAsync(command.RoomId, command.RoomNumber, command.RoomTypeId, command.Status);
    }
}

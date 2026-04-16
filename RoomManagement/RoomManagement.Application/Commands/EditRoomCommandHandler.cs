using MediatR;
using RoomManagement.Application.Services;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public class EditRoomCommandHandler : IRequestHandler<EditRoomCommand, Result<bool>>
{
    private readonly IRoomService _roomService;

    public EditRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<Result<bool>> Handle(EditRoomCommand request, CancellationToken cancellationToken)
    {
        return await _roomService.UpdateAsync(request.RoomId, request.RoomNumber, request.RoomTypeId, request.Status);
    }
}

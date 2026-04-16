using MediatR;
using RoomManagement.Application.Services;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public class AddRoomCommandHandler : IRequestHandler<AddRoomCommand, Result<Guid>>
{
    private readonly IRoomService _roomService;

    public AddRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<Result<Guid>> Handle(AddRoomCommand request, CancellationToken cancellationToken)
    {
        return await _roomService.AddAsync(request.RoomNumber, request.RoomTypeId);
    }
}

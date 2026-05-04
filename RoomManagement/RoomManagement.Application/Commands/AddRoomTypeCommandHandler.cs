using MediatR;
using RoomManagement.Application.Services;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public class AddRoomTypeCommandHandler : IRequestHandler<AddRoomTypeCommand, Result<Guid>>
{
    private readonly IRoomTypeService _roomTypeService;

    public AddRoomTypeCommandHandler(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    public async Task<Result<Guid>> Handle(AddRoomTypeCommand request, CancellationToken cancellationToken)
    {
        return await _roomTypeService.AddAsync(request.Name, request.Price);
    }
}

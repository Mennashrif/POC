using MediatR;
using RoomManagement.Application.Services;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public class EditRoomTypeCommandHandler : IRequestHandler<EditRoomTypeCommand, Result<bool>>
{
    private readonly IRoomTypeService _roomTypeService;

    public EditRoomTypeCommandHandler(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    public async Task<Result<bool>> Handle(EditRoomTypeCommand request, CancellationToken cancellationToken)
    {
        return await _roomTypeService.UpdateAsync(request.RoomTypeId, request.Name, request.Price);
    }
}

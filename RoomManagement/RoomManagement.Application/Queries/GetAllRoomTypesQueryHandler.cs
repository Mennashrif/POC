using MediatR;
using RoomManagement.Application.Abstractions;
using RoomManagement.Application.DTOs;

namespace RoomManagement.Application.Queries;

public class GetAllRoomTypesQueryHandler : IRequestHandler<GetAllRoomTypesQuery, List<RoomTypeDto>>
{
    private readonly IRoomTypeRepository _repository;

    public GetAllRoomTypesQueryHandler(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RoomTypeDto>> Handle(GetAllRoomTypesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}

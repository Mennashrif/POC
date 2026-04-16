using MediatR;
using RoomManagement.Application.Abstractions;
using RoomManagement.Application.DTOs;

namespace RoomManagement.Application.Queries;

public class GetAllRoomsQueryHandler : IRequestHandler<GetAllRoomsQuery, List<RoomDto>>
{
    private readonly IRoomRepository _repository;

    public GetAllRoomsQueryHandler(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RoomDto>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}

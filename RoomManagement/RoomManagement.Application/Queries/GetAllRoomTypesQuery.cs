using MediatR;
using RoomManagement.Application.DTOs;

namespace RoomManagement.Application.Queries;

public record GetAllRoomTypesQuery() : IRequest<List<RoomTypeDto>>;

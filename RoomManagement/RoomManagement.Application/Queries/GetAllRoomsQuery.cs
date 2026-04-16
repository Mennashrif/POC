using MediatR;
using RoomManagement.Application.DTOs;

namespace RoomManagement.Application.Queries;

public record GetAllRoomsQuery() : IRequest<List<RoomDto>>;

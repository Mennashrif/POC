using MediatR;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public record EditRoomTypeCommand(Guid RoomTypeId, string Name, decimal Price) : IRequest<Result<bool>>;

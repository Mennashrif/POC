using MediatR;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public record AddRoomTypeCommand(string Name, decimal Price) : IRequest<Result<Guid>>;

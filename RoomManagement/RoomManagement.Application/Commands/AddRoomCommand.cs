using MediatR;
using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Commands;

public record AddRoomCommand(string RoomNumber, Guid RoomTypeId) : IRequest<Result<Guid>>;

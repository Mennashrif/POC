using MediatR;
using RoomManagement.Domain.Abstractions;
using RoomManagement.Domain.Models;

namespace RoomManagement.Application.Commands;

public record EditRoomCommand(Guid RoomId, string RoomNumber, Guid RoomTypeId, RoomStatus Status) : IRequest<Result<bool>>;

using System;
using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands;

public record CheckInReservationCommand(Guid ReservationId, List<string> PhysicalRoomIds) : IRequest<Result<bool>>;

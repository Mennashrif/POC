using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using MediatR;

namespace Booking.Application.Commands;

public record UpdateReservationCommand(
    Guid ReservationId,
    GuestDetails Guest,
    DateTime CheckIn,
    DateTime CheckOut
) : IRequest<Result<bool>>;

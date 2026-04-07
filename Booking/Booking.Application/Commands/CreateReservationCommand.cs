using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using MediatR;

namespace Booking.Application.Commands;

public record CreateReservationCommand(
    GuestDetails Guest,
    DateTime CheckIn,
    DateTime CheckOut,
    List<RoomRequest> RoomRequests
) : IRequest<Result<Guid>>;

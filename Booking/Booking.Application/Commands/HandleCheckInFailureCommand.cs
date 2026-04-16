using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands
{
    public record HandleCheckInFailureCommand(
        Guid ReservationId,
        List<string> PhysicalRoomNumbers,
        string Reason,
        DateTime OccurredAt
    ) : IRequest<Result<bool>>;
}

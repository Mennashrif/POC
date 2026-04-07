using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Services;
using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands;

public class CheckInReservationCommandHandler : IRequestHandler<CheckInReservationCommand, Result<bool>>
{
    private readonly IReservationService _reservationService;

    public CheckInReservationCommandHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<Result<bool>> Handle(CheckInReservationCommand request, CancellationToken cancellationToken)
    {
        return await _reservationService.CheckInAsync(request.ReservationId, request.PhysicalRoomIds);
    }
}

using System.Threading;
using System.Threading.Tasks;
using Booking.Application.DTOs;
using Booking.Application.Services;
using MediatR;

namespace Booking.Application.Queries;

// Thin dispatcher — no business logic here.
// All read orchestration is delegated to IReservationService.
public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IReservationService _reservationService;

    public GetReservationByIdQueryHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _reservationService.GetByIdAsync(request.Id);
    }
}

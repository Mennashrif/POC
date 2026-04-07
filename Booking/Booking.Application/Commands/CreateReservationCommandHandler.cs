using System;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Services;
using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands;

// Thin dispatcher — no business logic here.
// All orchestration is delegated to IReservationService.
public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Result<Guid>>
{
    private readonly IReservationService _reservationService;

    public CreateReservationCommandHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<Result<Guid>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        return await _reservationService.CreateAsync(
            request.Guest,
            request.CheckIn,
            request.CheckOut,
            request.RoomRequests);
    }
}

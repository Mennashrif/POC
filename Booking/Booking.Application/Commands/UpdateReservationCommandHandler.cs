using Booking.Application.Services;
using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands;

public class UpdateReservationCommandHandler : IRequestHandler<UpdateReservationCommand, Result<bool>>
{
    private readonly IReservationService _reservationService;

    public UpdateReservationCommandHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<Result<bool>> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
    {
        return await _reservationService.UpdateAsync(
            request.ReservationId,
            request.Guest,
            request.CheckIn,
            request.CheckOut);
    }
}

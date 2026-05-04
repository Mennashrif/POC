using Booking.Application.Services;
using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands
{
    public class HandleCheckInFailureCommandHandler : IRequestHandler<HandleCheckInFailureCommand, Result<bool>>
    {
        private readonly IReservationService _reservationService;

        public HandleCheckInFailureCommandHandler(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task<Result<bool>> Handle(HandleCheckInFailureCommand request, CancellationToken cancellationToken)
            => await _reservationService.HandleCheckInFailureAsync(request.ReservationId, request.Reason);
    }
}

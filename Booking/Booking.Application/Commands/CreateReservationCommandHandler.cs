using System;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Application.Services;
using Booking.Domain.Abstractions;
using MediatR;

namespace Booking.Application.Commands;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Result<Guid>>
{
    private readonly IReservationService    _reservationService;
    private readonly IReservationNotifier   _notifier;

    public CreateReservationCommandHandler(
        IReservationService reservationService,
        IReservationNotifier notifier)
    {
        _reservationService = reservationService;
        _notifier           = notifier;
    }

    public async Task<Result<Guid>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var result = await _reservationService.CreateAsync(
            request.Guest,
            request.CheckIn,
            request.CheckOut,
            request.RoomRequests);

        if (result.IsSuccess)
            await _notifier.NotifyReservationCreatedAsync(
                result.Value,
                request.Guest.Name,
                request.CheckIn.ToString("yyyy-MM-dd"),
                request.CheckOut.ToString("yyyy-MM-dd"));

        return result;
    }
}

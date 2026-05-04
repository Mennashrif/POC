using Booking.Api.Hubs;
using Booking.Application.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Booking.Api.Notifications;

public class SignalRReservationNotifier : IReservationNotifier
{
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRReservationNotifier(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public Task NotifyReservationCreatedAsync(Guid reservationId, string guestName, string checkIn, string checkOut)
        => _hub.Clients.All.SendAsync("ReservationCreated", new
        {
            ReservationId = reservationId,
            GuestName     = guestName,
            CheckIn       = checkIn,
            CheckOut      = checkOut
        });
}

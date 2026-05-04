namespace Booking.Application.Abstractions;

public interface IReservationNotifier
{
    Task NotifyReservationCreatedAsync(Guid reservationId, string guestName, string checkIn, string checkOut);
}

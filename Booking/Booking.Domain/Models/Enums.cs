namespace Booking.Domain.Models;

public enum ReservationStatus :byte
{
    Pending,
    Confirmed,
    CheckedIn,
    CheckedOut,
    Cancelled
}


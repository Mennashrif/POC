namespace Booking.Domain.Models;

public enum ReservationStatus
{
    Pending,
    Confirmed,
    CheckedIn,
    CheckedOut,
    Cancelled
}

public enum RoomTypeEnum
{
    Single,
    Double,
    Suite
}

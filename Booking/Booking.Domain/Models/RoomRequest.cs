namespace Booking.Domain.Models;

// This is a Value Object (V) representing the guest's initial request.
// It is NOT a specific physical room Entity (which would include the actual room number).
public record RoomRequest(RoomTypeEnum RoomType, int Quantity = 1)
{
    // The guest requests "1 Double Room".
    // At Check-in, the system looks at this request and assigns a specific Room Entity.
}

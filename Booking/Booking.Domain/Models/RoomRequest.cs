namespace Booking.Domain.Models;

public record RoomRequest(Guid RoomTypeId, int Quantity = 1)
{
}

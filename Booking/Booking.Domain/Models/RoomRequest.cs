namespace Booking.Domain.Models;

public record RoomRequest(RoomTypeEnum RoomType, int Quantity = 1)
{
}

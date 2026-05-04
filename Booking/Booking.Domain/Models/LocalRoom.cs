using Booking.Domain.Abstractions;

namespace Booking.Domain.Models;

public class LocalRoom : BaseEntity<Guid>
{
    public string RoomNumber { get; private set; }
    public Guid RoomTypeId { get; private set; }
    public string Status { get; private set; }

    private LocalRoom() : base(Guid.NewGuid()) { }

    public LocalRoom(Guid roomId, string roomNumber, Guid roomTypeId, string status)
        : base(roomId)
    {
        RoomNumber = roomNumber;
        RoomTypeId = roomTypeId;
        Status = status;
    }

    public void Update(string roomNumber, Guid roomTypeId, string status)
    {
        RoomNumber = roomNumber;
        RoomTypeId = roomTypeId;
        Status = status;
    }
}

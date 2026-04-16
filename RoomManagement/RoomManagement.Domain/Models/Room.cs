using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Domain.Models;

public class Room : BaseEntity<Guid>
{
    public string RoomNumber { get; private set; }
    public Guid RoomTypeId { get; private set; }
    public RoomType? RoomType { get; private set; }
    public RoomStatus RoomStatus { get; private set; }

    private Room() : base(Guid.NewGuid()) { }

    public Room(string roomNumber, Guid roomTypeId) : base(Guid.NewGuid())
    {
        RoomNumber = roomNumber;
        RoomTypeId = roomTypeId;
        RoomStatus = RoomStatus.Available;

    }

    public void Update(string roomNumber, Guid roomTypeId)
    {
        RoomNumber = roomNumber;
        RoomTypeId = roomTypeId;
    }

    public void SetMaintenance()
    {
        RoomStatus = RoomStatus.Maintenance;
    }

    public void SetAvailable()
    {
        RoomStatus = RoomStatus.Available;
    }

    public bool CanBeOccupied() => RoomStatus == RoomStatus.Available;

    public void SetOccupied()
    {
        RoomStatus = RoomStatus.Occupied;
    }
}

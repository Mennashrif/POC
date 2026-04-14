using System;
using Booking.Domain.Abstractions;
using Booking.Domain.Events;

namespace Booking.Domain.Models;

public class Reservation : BaseEntity<Guid>, IAggregateRoot
{
    public ReservationStatus Status { get; private set; }
    
    public StayDate StayDate { get; private set; }
    
    public GuestDetails Guest { get; private set; }
    
    private readonly List<RoomRequest> _roomRequests = new();
    public IReadOnlyCollection<RoomRequest> RoomRequests => _roomRequests.AsReadOnly();
    
    private readonly List<string> _assignedPhysicalRoomIds = new();
    public IReadOnlyCollection<string> AssignedPhysicalRoomIds => _assignedPhysicalRoomIds.AsReadOnly();
    public byte[] RowVersion { get; set; }
    public Reservation() : base(Guid.NewGuid()) { }
    public Reservation(GuestDetails guest, StayDate stayDate, IEnumerable<RoomRequest> roomRequests) : base(Guid.NewGuid())
    {
        Guest = guest ?? throw new ArgumentNullException(nameof(guest));
        StayDate = stayDate;
        _roomRequests.AddRange(roomRequests);
        Status = ReservationStatus.Pending;
    }

    public void Confirm()
    {
        if(Status != ReservationStatus.Pending)
            throw new InvalidOperationException("Reservation is not in Pending state");

        Status = ReservationStatus.Confirmed;

   }

   public void CheckIn(List<string> physicalRoomIds)
   {
        if(Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Reservation is not in Confirmed state");

        if (physicalRoomIds.Count != _roomRequests.Sum(r => r.Quantity))
            throw new ArgumentException("Number of physical rooms must match the total quantity requested.");

        Status = ReservationStatus.CheckedIn;
        _assignedPhysicalRoomIds.AddRange(physicalRoomIds);

   }

   public void CheckOut()
   {
        if(Status != ReservationStatus.CheckedIn)
            throw new InvalidOperationException("Reservation is not in CheckedIn state");

        Status = ReservationStatus.CheckedOut;
   }

   public void Update(GuestDetails guest, StayDate stayDate)
   {
        if (Status != ReservationStatus.Pending && Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Only Pending or Confirmed reservations can be updated.");

        Guest = guest ?? throw new ArgumentNullException(nameof(guest));
        StayDate = stayDate;
   }

   public void Cancel()
   {
        if(Status != ReservationStatus.Pending && Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Reservation is not in Pending or Confirmed state");

        Status = ReservationStatus.Cancelled;
   }
}

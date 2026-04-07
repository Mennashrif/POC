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

    private Reservation() : base(Guid.Empty) { } // Required by EF Core for materialization

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

        AddDomainEvent(new ReservationConfirmedEvent(
            Id,
            StayDate.CheckIn,
            StayDate.CheckOut,
            Guest.Name,
            DateTime.UtcNow
        ));
   }

   public void CheckIn(List<string> physicalRoomIds)
   {
        if(Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Reservation is not in Confirmed state");

        if (physicalRoomIds.Count != _roomRequests.Sum(r => r.Quantity))
            throw new ArgumentException("Number of physical rooms must match the total quantity requested.");

        Status = ReservationStatus.CheckedIn;
        _assignedPhysicalRoomIds.AddRange(physicalRoomIds);

        AddDomainEvent(new ReservationCheckedInEvent(
            Id,
            physicalRoomIds,
            Guest.Name,
            DateTime.UtcNow,
            DateTime.UtcNow
        ));
   }

   public void CheckOut()
   {
        if(Status != ReservationStatus.CheckedIn)
            throw new InvalidOperationException("Reservation is not in CheckedIn state");

        Status = ReservationStatus.CheckedOut;
   }

   public void Cancel()
   {
        if(Status != ReservationStatus.Pending && Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Reservation is not in Pending or Confirmed state");
            
        Status = ReservationStatus.Cancelled;
   }
}

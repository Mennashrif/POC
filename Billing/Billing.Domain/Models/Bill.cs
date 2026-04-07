using System;
using System.Collections.Generic;
using Billing.Domain.Abstractions;

namespace Billing.Domain.Models;

public class Bill : BaseEntity<Guid>, IAggregateRoot
{
    public Guid ReservationId { get; private set; }
    public DateTime CheckInDate { get; private set; }
    public BillStatus Status { get; private set; }

    public string GuestName { get; private set; }

    private readonly List<string> _physicalRoomIds = new();
    public IReadOnlyCollection<string> PhysicalRoomIds => _physicalRoomIds.AsReadOnly();

    private Bill() : base(Guid.Empty) { } // Required by EF Core

    public Bill(Guid reservationId, string guestName, List<string> physicalRoomIds, DateTime checkInDate)
        : base(Guid.NewGuid())
    {
        ReservationId = reservationId;
        GuestName = guestName;
        _physicalRoomIds.AddRange(physicalRoomIds);
        CheckInDate = checkInDate;
        Status = BillStatus.Open;
    }

    public void MarkAsPaid()
    {
        if (Status != BillStatus.Open)
            throw new InvalidOperationException("Only open bills can be marked as paid.");

        Status = BillStatus.Paid;
    }

    public void Cancel()
    {
        if (Status != BillStatus.Open)
            throw new InvalidOperationException("Only open bills can be cancelled.");

        Status = BillStatus.Cancelled;
    }
}

using System;
using System.Collections.Generic;

namespace Booking.Domain.Events;

public record ReservationCheckedInEvent(
    Guid ReservationId,
    List<string> PhysicalRoomIds,
    string GuestName,
    DateTime CheckInDate,
    DateTime OccurredAt
);

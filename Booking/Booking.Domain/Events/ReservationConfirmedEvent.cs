using System;
using System.Collections.Generic;

namespace Booking.Domain.Events;

public record ReservationConfirmedEvent(
    Guid ReservationId,
    DateTime CheckIn,
    DateTime CheckOut,
    string GuestName,
    DateTime OccurredAt
);

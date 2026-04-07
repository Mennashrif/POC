using System;
using System.Collections.Generic;

namespace Booking.Application.DTOs;

public record ReservationDto(
    Guid Id,
    string Status,
    DateTime CheckIn,
    DateTime CheckOut,
    string GuestName,
    int TotalRoomsRequested
);

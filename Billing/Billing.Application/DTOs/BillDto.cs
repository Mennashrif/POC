using System;
using System.Collections.Generic;

namespace Billing.Application.DTOs;

public record BillDto(
    Guid Id,
    Guid ReservationId,
    List<string> PhysicalRoomIds,
    string GuestName,
    DateTime CheckInDate,
    string Status
);

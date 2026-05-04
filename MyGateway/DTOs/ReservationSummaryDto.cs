namespace MyGateway.DTOs;

public record ReservationSummaryDto(
    Guid ReservationId,
    string GuestName,
    string Status,
    DateTime CheckIn,
    DateTime CheckOut,
    int TotalRoomsRequested,
    BillSummaryDto? Bill
);

public record BillSummaryDto(
    Guid BillId,
    List<string> PhysicalRoomIds,
    string BillStatus
);

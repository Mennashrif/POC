namespace MyGateway.DTOs.Downstream;

public record BillDto(
    Guid Id,
    Guid ReservationId,
    List<string> PhysicalRoomIds,
    string GuestName,
    string Status
);

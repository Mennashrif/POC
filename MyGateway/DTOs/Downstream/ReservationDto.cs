namespace MyGateway.DTOs.Downstream;

public record ReservationDto(
    Guid Id,
    string Status,
    DateTime CheckIn,
    DateTime CheckOut,
    string GuestName,
    int TotalRoomsRequested
);

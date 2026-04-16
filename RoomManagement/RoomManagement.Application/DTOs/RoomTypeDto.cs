namespace RoomManagement.Application.DTOs;

public record RoomTypeDto(
    Guid Id,
    string Name,
    decimal Price
);

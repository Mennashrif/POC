namespace RoomManagement.Application.DTOs;

public record RoomDto(
    Guid Id,
    string RoomNumber,
    Guid RoomTypeId,
    string RoomTypeName,
    decimal Price,
    string Status
);

namespace RoomManagement.Domain.Events;

public record RoomAddedEvent(
    Guid RoomId,
    string RoomNumber,
    Guid RoomTypeId,
    DateTime OccurredAt
);

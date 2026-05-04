namespace RoomManagement.Domain.Events;

public record RoomStatusChangedEvent(
    Guid RoomId,
    string RoomNumber,
    string OldStatus,
    string NewStatus,
    DateTime OccurredAt
);

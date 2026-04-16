namespace RoomManagement.Domain.Events;

public record RoomMaintenanceScheduledEvent(
    Guid RoomId,
    string RoomNumber,
    DateTime OccurredAt
);

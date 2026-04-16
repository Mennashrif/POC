using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Domain.Models;

public class ProcessedEvent : BaseEntity<Guid>
{
    public string EventId { get; private set; }
    public string EventType { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    private ProcessedEvent() : base(Guid.NewGuid()) { }

    public ProcessedEvent(string eventId, string eventType) : base(Guid.NewGuid())
    {
        EventId = eventId;
        EventType = eventType;
        ProcessedAt = DateTime.UtcNow;
    }
}

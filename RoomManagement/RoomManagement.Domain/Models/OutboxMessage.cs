using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Domain.Models;

public class OutboxMessage : BaseEntity<Guid>
{
    public string EventType { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    private OutboxMessage() : base(Guid.NewGuid()) { }

    public OutboxMessage(string eventType, string payload) : base(Guid.NewGuid())
    {
        EventType = eventType;
        Payload = payload;
        OccurredAt = DateTime.UtcNow;
        PublishedAt = null;
    }

    public void MarkAsPublished()
    {
        PublishedAt = DateTime.UtcNow;
    }
}

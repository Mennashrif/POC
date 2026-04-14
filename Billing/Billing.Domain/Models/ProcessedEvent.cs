using Billing.Domain.Abstractions;

namespace Billing.Domain.Models;

public class ProcessedEvent : BaseEntity<Guid>
{
    public Guid ReservationId { get; private set; }
    public string EventType { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    private ProcessedEvent() : base(Guid.Empty) { }

    public ProcessedEvent(Guid reservationId, string eventType) : base(Guid.NewGuid())
    {
        ReservationId = reservationId;
        EventType = eventType;
        ProcessedAt = DateTime.UtcNow;
    }
}

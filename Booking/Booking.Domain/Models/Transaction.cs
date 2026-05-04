using Booking.Domain.Abstractions;

namespace Booking.Domain.Models
{
    public class Transaction : BaseEntity<Guid>
    {
        public string EventType { get; private set; }
        public string Payload { get; private set; }
        public DateTime OccurredAt { get; private set; }
        public DateTime? PublishedAt { get; private set; }

        public Transaction() : base(Guid.NewGuid()){}
        public Transaction(string eventType, string payload) : base(Guid.NewGuid())
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
}

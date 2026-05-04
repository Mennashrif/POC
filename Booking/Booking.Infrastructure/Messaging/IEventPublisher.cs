using System.Threading;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken = default);
}

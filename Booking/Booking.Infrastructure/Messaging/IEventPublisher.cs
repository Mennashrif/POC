using System.Threading;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default);
}

using Booking.Application.Abstractions;
using Booking.Infrastructure.Messaging;

namespace Booking.Infrastructure.Jobs;

public class OutboxProcessorJob
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IEventPublisher _eventPublisher;

    public OutboxProcessorJob(
        ITransactionRepository transactionRepository,
        IEventPublisher eventPublisher)
    {
        _transactionRepository = transactionRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task ProcessAsync()
    {
        var unpublished = await _transactionRepository.GetUnpublishedAsync();

        if (!unpublished.Any())
            return;

        foreach (var transaction in unpublished) 
        {
            await _eventPublisher.PublishAsync(transaction.EventType, transaction.Payload);
            transaction.MarkAsPublished();
        }

        await _transactionRepository.SaveChangesAsync();
    }
}

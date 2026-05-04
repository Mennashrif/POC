using Booking.Application.Abstractions;
using Booking.Infrastructure.Messaging;

namespace Booking.Infrastructure.Jobs;

public class OutboxProcessorJob
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public OutboxProcessorJob(
        ITransactionRepository transactionRepository,
        IEventPublisher eventPublisher,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _eventPublisher = eventPublisher;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.SaveChangesAsync();
    }
}

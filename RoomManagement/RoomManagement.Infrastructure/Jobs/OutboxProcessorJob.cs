using RoomManagement.Application.Abstractions;
using RoomManagement.Infrastructure.Messaging;

namespace RoomManagement.Infrastructure.Jobs;

public class OutboxProcessorJob
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public OutboxProcessorJob(
        IOutboxRepository outboxRepository,
        IEventPublisher eventPublisher,
        IUnitOfWork unitOfWork)
    {
        _outboxRepository = outboxRepository;
        _eventPublisher = eventPublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessAsync()
    {
        var unpublished = await _outboxRepository.GetUnpublishedAsync();

        if (!unpublished.Any())
            return;

        foreach (var message in unpublished)
        {
            await _eventPublisher.PublishAsync(message.EventType, message.Payload);
            message.MarkAsPublished();
        }

        await _unitOfWork.SaveChangesAsync();
    }
}

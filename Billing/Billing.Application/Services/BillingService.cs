using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Billing.Application.Abstractions;
using Billing.Application.DTOs;
using Billing.Domain.Abstractions;
using Billing.Domain.Models;

namespace Billing.Application.Services;

public class BillingService : IBillingService
{
    private readonly IBillRepository _repository;
    private readonly IProcessedEventRepository _processedEventRepository;

    public BillingService(IBillRepository repository, IProcessedEventRepository processedEventRepository)
    {
        _repository = repository;
        _processedEventRepository = processedEventRepository;
    }

    public async Task<Result<Guid>> CreateBillAsync(
        Guid reservationId,
        string guestName,
        List<string> physicalRoomIds,
        DateTime checkInDate)
    {
        // Idempotency check — if this event was already processed, skip it
        var alreadyProcessed = await _processedEventRepository.ExistsAsync(reservationId, "ReservationCheckedInEvent");
        if (alreadyProcessed)
            return Result<Guid>.Success(Guid.Empty); // already handled, not an error

        var bill = new Bill(reservationId, guestName, physicalRoomIds, checkInDate);
        var processedEvent = new ProcessedEvent(reservationId, "ReservationCheckedInEvent");

        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        await _repository.AddAsync(bill);
        await _repository.SaveChangesAsync();

        await _processedEventRepository.AddAsync(processedEvent);
        await _processedEventRepository.SaveChangesAsync();

        scope.Complete();

        return Result<Guid>.Success(bill.Id);
    }

    public async Task<BillDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetDetailsByIdAsync(id);
    }

    public async Task<BillDto?> GetByReservationIdAsync(Guid reservationId)
    {
        return await _repository.GetByReservationIdAsync(reservationId);
    }
}

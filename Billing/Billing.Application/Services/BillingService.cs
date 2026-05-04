using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Billing.Application.Abstractions;
using Billing.Application.DTOs;
using Billing.Domain.Abstractions;
using Billing.Domain.Models;

namespace Billing.Application.Services;

public class BillingService : IBillingService
{
    private readonly IBillRepository _repository;
    private readonly IProcessedEventRepository _processedEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BillingService(
        IBillRepository repository,
        IProcessedEventRepository processedEventRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _processedEventRepository = processedEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> CreateBillAsync(
        Guid reservationId,
        string guestName,
        List<string> physicalRoomIds,
        DateTime checkInDate)
    {
        var alreadyProcessed = await _processedEventRepository.ExistsAsync(reservationId, "ReservationCheckedInEvent");
        if (alreadyProcessed)
            return Result<Guid>.Success(Guid.Empty);

        var bill = new Bill(reservationId, guestName, physicalRoomIds, checkInDate);
        var processedEvent = new ProcessedEvent(reservationId, "ReservationCheckedInEvent");

        await _repository.AddAsync(bill);
        await _processedEventRepository.AddAsync(processedEvent);

        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(bill.Id);
    }

    public async Task<Result<bool>> CancelBillAsync(Guid reservationId)
    {
        var alreadyProcessed = await _processedEventRepository.ExistsAsync(reservationId, "CheckInRevertedEvent");
        if (alreadyProcessed)
            return Result<bool>.Success(true);

        var bill = await _repository.FindByReservationIdAsync(reservationId);
        if (bill is null)
            return Result<bool>.Failure($"No bill found for reservation {reservationId}.");

        bill.Cancel();

        await _processedEventRepository.AddAsync(new ProcessedEvent(reservationId, "CheckInRevertedEvent"));

        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
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

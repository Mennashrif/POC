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

    public BillingService(IBillRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> CreateBillAsync(
        Guid reservationId,
        string guestName,
        List<string> physicalRoomIds,
        DateTime checkInDate)
    {
        var bill = new Bill(reservationId, guestName, physicalRoomIds, checkInDate);

        await _repository.AddAsync(bill);
        await _repository.SaveChangesAsync();

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

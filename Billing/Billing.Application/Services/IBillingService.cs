using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Billing.Application.DTOs;
using Billing.Domain.Abstractions;

namespace Billing.Application.Services;

public interface IBillingService
{
    Task<Result<Guid>> CreateBillAsync(Guid reservationId, string guestName, List<string> physicalRoomIds, DateTime checkInDate);

    /// <summary>
    /// Saga compensation: cancels the bill when Booking reports that check-in was reverted.
    /// </summary>
    Task<Result<bool>> CancelBillAsync(Guid reservationId);

    Task<BillDto?> GetByIdAsync(Guid id);
    Task<BillDto?> GetByReservationIdAsync(Guid reservationId);
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Billing.Application.DTOs;
using Billing.Domain.Abstractions;

namespace Billing.Application.Services;

public interface IBillingService
{
    Task<Result<Guid>> CreateBillAsync(Guid reservationId, string guestName, List<string> physicalRoomIds, DateTime checkInDate);
    Task<BillDto?> GetByIdAsync(Guid id);
    Task<BillDto?> GetByReservationIdAsync(Guid reservationId);
}

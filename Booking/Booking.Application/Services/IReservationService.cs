using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Application.DTOs;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;

namespace Booking.Application.Services;

// Application Service — sits between MediatR Handlers and the Repository.
// Handlers are thin dispatchers; all orchestration logic lives here.
public interface IReservationService
{
    // Write side: enforces domain rules, creates the aggregate, persists it.
    Task<Result<Guid>> CreateAsync(
        GuestDetails guest,
        DateTime checkIn,
        DateTime checkOut,
        List<RoomRequest> roomRequests);

    Task<Result<bool>> CheckInAsync(Guid reservationId, List<string> physicalRoomIds);

    /// <summary>
    /// Saga compensation: reverts a CheckedIn reservation back to Confirmed
    /// and stages a CheckInRevertedEvent so Billing can cancel the bill.
    /// </summary>
    Task<Result<bool>> HandleCheckInFailureAsync(Guid reservationId, string reason);

    // Read side: returns a flat DTO — never a domain object.
    Task<ReservationDto?> GetByIdAsync(Guid id);
}

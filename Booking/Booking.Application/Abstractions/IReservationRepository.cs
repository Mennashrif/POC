using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Application.DTOs;
using Booking.Domain.Models;

namespace Booking.Application.Abstractions;

// Write-side repository — specific to the Reservation Aggregate Root.
// Only exposes what Command handlers and Services actually need.
public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<List<ReservationDto>> GetAllAsync();
    Task<List<Reservation>> GetOverlappingAsync(StayDate dates);
    Task<ReservationDto?> GetDetailsByIdAsync(Guid id);
    Task AddAsync(Reservation reservation);
    void Update(Reservation reservation);
    Task SaveChangesAsync();
}

using Booking.Application.Abstractions;
using Booking.Application.DTOs;
using Booking.Domain.Models;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly BookingDbContext _dbContext;

    public ReservationRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Reservation?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Reservations.FindAsync(id);
    }

    public async Task<List<Reservation>> GetOverlappingAsync(StayDate dates)
    {
        return await _dbContext.Reservations
            .Where(r => r.Status != ReservationStatus.Cancelled)
            .Where(r => r.StayDate.CheckIn < dates.CheckOut && r.StayDate.CheckOut > dates.CheckIn)
            .ToListAsync();
    }

    public async Task<List<ReservationDto>> GetAllAsync()
    {
        return await _dbContext.Reservations
            .AsNoTracking()
            .Select(r => new ReservationDto(
                r.Id,
                r.Status.ToString(),
                r.StayDate.CheckIn,
                r.StayDate.CheckOut,
                r.Guest.Name,
                r.RoomRequests.Sum(req => req.Quantity)
            ))
            .ToListAsync();
    }

    public async Task<ReservationDto?> GetDetailsByIdAsync(Guid id)
    {
        return await _dbContext.Reservations
            .AsNoTracking()
            //.Where(r => r.Id == id)
            .Select(r => new ReservationDto(
                r.Id,
                r.Status.ToString(),
                r.StayDate.CheckIn,
                r.StayDate.CheckOut,
                r.Guest.Name,
                r.RoomRequests.Sum(req => req.Quantity)
            ))
            .FirstOrDefaultAsync();
    }

    public void Add(Reservation reservation)
    {
        _dbContext.Reservations.Add(reservation);
    }

}

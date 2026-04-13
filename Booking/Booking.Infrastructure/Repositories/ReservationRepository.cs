using Booking.Application.Abstractions;
using Booking.Application.DTOs;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Booking.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly BookingDbContext _dbContext;
    private readonly IEventPublisher _eventPublisher;

    public ReservationRepository(BookingDbContext dbContext, IEventPublisher eventPublisher)
    {
        _dbContext = dbContext;
        _eventPublisher = eventPublisher;
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

    public async Task AddAsync(Reservation reservation)
    {
        await _dbContext.Reservations.AddAsync(reservation);
    }

    public void Update(Reservation reservation)
    { 
        _dbContext.Reservations.Update(reservation);
    }

    public async Task SaveChangesAsync()
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<BaseEntity<Guid>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        await _dbContext.SaveChangesAsync();

        foreach (var domainEvent in domainEvents)
            await _eventPublisher.PublishAsync(domainEvent);

        foreach (var entry in domainEntities)
            entry.Entity.ClearDomainEvents();
    }
}

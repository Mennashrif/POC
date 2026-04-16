using System;
using System.Linq;
using System.Threading.Tasks;
using Billing.Application.Abstractions;
using Billing.Application.DTOs;
using Billing.Domain.Models;
using Billing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure.Repositories;

public class BillRepository : IBillRepository
{
    private readonly BillingDbContext _dbContext;

    public BillRepository(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Bill?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Bills.FindAsync(id);
    }

    public async Task<BillDto?> GetDetailsByIdAsync(Guid id)
    {
        return await _dbContext.Bills
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BillDto(
                b.Id,
                b.ReservationId,
                b.PhysicalRoomIds.ToList(),
                b.GuestName,
                b.CheckInDate,
                b.Status.ToString()
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<BillDto?> GetByReservationIdAsync(Guid reservationId)
    {
        return await _dbContext.Bills
            .AsNoTracking()
            .Where(b => b.ReservationId == reservationId)
            .Select(b => new BillDto(
                b.Id,
                b.ReservationId,
                b.PhysicalRoomIds.ToList(),
                b.GuestName,
                b.CheckInDate,
                b.Status.ToString()
            ))
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Bill bill)
    {
        await _dbContext.Bills.AddAsync(bill);
    }

    public Task<Bill?> FindByReservationIdAsync(Guid reservationId)
    {
        return _dbContext.Bills.FirstOrDefaultAsync(b => b.ReservationId == reservationId);
    }
}

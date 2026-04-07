using System;
using System.Threading.Tasks;
using Billing.Application.DTOs;
using Billing.Domain.Models;

namespace Billing.Application.Abstractions;

public interface IBillRepository
{
    Task<Bill?> GetByIdAsync(Guid id);
    Task<BillDto?> GetDetailsByIdAsync(Guid id);
    Task<BillDto?> GetByReservationIdAsync(Guid reservationId);
    Task AddAsync(Bill bill);
    Task SaveChangesAsync();
}

using Booking.Domain.Models;

namespace Booking.Application.Abstractions
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<List<Transaction>> GetUnpublishedAsync();
        Task SaveChangesAsync();
    }
}
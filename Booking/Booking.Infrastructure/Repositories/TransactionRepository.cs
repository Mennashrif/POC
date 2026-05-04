using Booking.Application.Abstractions;
using Booking.Domain.Models;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BookingDbContext _dbContext;

        public TransactionRepository(BookingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// This for test that the transaction will rollback 
        //public Task AddAsync(Transaction transaction)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task AddAsync(Transaction transaction)
        {

            await _dbContext.Transactions.AddAsync(transaction);
        }

        public async Task<List<Transaction>> GetUnpublishedAsync()
        {
            return await _dbContext.Transactions
                .Where(t => t.PublishedAt == null)
                .OrderBy(t => t.OccurredAt)
                .ToListAsync();
        }

    }
}

using RoomManagement.Application.Abstractions;
using RoomManagement.Infrastructure.Data;

namespace RoomManagement.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly RoomManagementDbContext _dbContext;

    public UnitOfWork(RoomManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}

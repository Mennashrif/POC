using Dapper;
using Microsoft.EntityFrameworkCore;
using RoomManagement.Application.Abstractions;
using RoomManagement.Application.DTOs;
using RoomManagement.Domain.Models;
using RoomManagement.Infrastructure.Data;

namespace RoomManagement.Infrastructure.Repositories;

public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly RoomManagementDbContext _dbContext;
    private readonly ISqlConnectionFactory _connectionFactory;

    public RoomTypeRepository(RoomManagementDbContext dbContext, ISqlConnectionFactory connectionFactory)
    {
        _dbContext = dbContext;
        _connectionFactory = connectionFactory;
    }

    // ── Reads via Dapper ─────────────────────────────────────────────────────

    public async Task<List<RoomTypeDto>> GetAllAsync()
    {
        const string sql = """
            SELECT rt.Id, rt.Name, rt.Price
            FROM   RoomTypes rt
            ORDER BY rt.Name
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<RoomTypeDto>(sql);
        return rows.ToList();
    }

    public async Task<RoomType?> GetByIdAsync(Guid id)
    {
        return await _dbContext.RoomTypes.FindAsync(id);
    }

    // ── Writes via EF Core ───────────────────────────────────────────────────

    public async Task AddAsync(RoomType roomType)
    {
        await _dbContext.RoomTypes.AddAsync(roomType);
    }
}

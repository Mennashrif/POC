using Dapper;
using RoomManagement.Application.Abstractions;
using RoomManagement.Application.DTOs;
using RoomManagement.Domain.Models;
using RoomManagement.Infrastructure.Data;

namespace RoomManagement.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly RoomManagementDbContext _dbContext;
    private readonly ISqlConnectionFactory _connectionFactory;

    public RoomRepository(RoomManagementDbContext dbContext, ISqlConnectionFactory connectionFactory)
    {
        _dbContext = dbContext;
        _connectionFactory = connectionFactory;
    }

    // ── Reads via Dapper ─────────────────────────────────────────────────────

    public async Task<List<RoomDto>> GetAllAsync()
    {
        const string sql = """
            SELECT r.Id,
                   r.RoomNumber,
                   r.RoomTypeId,
                   rt.Name AS RoomTypeName,
                   rt.Price,
                   CASE r.RoomStatus
                       WHEN 0 THEN 'Available'
                       WHEN 1 THEN 'Occupied'
                       WHEN 2 THEN 'Maintenance'
                   END AS Status
            FROM   Rooms r
            INNER JOIN RoomTypes rt ON rt.Id = r.RoomTypeId
            ORDER BY r.RoomNumber
            """;

        using var connection = _connectionFactory.CreateConnection();
        return (await connection.QueryAsync<RoomDto>(sql)).ToList();
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT Id, RoomNumber, RoomTypeId, RoomStatus FROM Rooms WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<RoomEntityRow>(sql, new { Id = id });

        return row is null ? null : new Room(row.Id, row.RoomNumber, row.RoomTypeId, (RoomStatus)row.RoomStatus);
    }

    public async Task<Room?> GetByRoomNumberAsync(string roomNumber)
    {
        const string sql = "SELECT Id, RoomNumber, RoomTypeId, RoomStatus FROM Rooms WHERE RoomNumber = @RoomNumber";

        using var connection = _connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<RoomEntityRow>(sql, new { RoomNumber = roomNumber });

        return row is null ? null : new Room(row.Id, row.RoomNumber, row.RoomTypeId, (RoomStatus)row.RoomStatus);
    }

    public async Task<List<Room>> GetByRoomNumbersAsync(List<string> physicalRoomNumbers)
    {
        if (physicalRoomNumbers.Count == 0) return [];

        const string sql = "SELECT Id, RoomNumber, RoomTypeId, RoomStatus FROM Rooms WHERE RoomNumber IN @RoomNumbers";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<RoomEntityRow>(sql, new { RoomNumbers = physicalRoomNumbers });

        return rows.Select(r => new Room(r.Id, r.RoomNumber, r.RoomTypeId, (RoomStatus)r.RoomStatus)).ToList();
    }

    // ── Writes via EF Core ───────────────────────────────────────────────────

    public async Task AddAsync(Room room)
    {
        await _dbContext.Rooms.AddAsync(room);
    }

    public void Update(Room room)
    {
        _dbContext.Rooms.Update(room);
    }

    public void UpdateRange(List<Room> rooms)
    {
        _dbContext.Rooms.UpdateRange(rooms);
    }

    // Used by entity reads — flat mirror of the Rooms table columns
    private sealed record RoomEntityRow(Guid Id, string RoomNumber, Guid RoomTypeId, byte RoomStatus);
}

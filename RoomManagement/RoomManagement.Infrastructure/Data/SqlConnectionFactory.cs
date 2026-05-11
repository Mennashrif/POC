using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RoomManagement.Infrastructure.Data;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("RoomManagementDb")
            ?? throw new InvalidOperationException("Missing connection string 'RoomManagementDb'.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}

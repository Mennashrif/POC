# WIP: Dapper Integration — RoomManagement

## Goal
Replace EF Core raw SQL / repository implementations in RoomManagement with Dapper for lightweight, explicit data access.

## Commits

### Add Dapper Package (7f9b611)
- Added `Dapper` NuGet package to `RoomManagement.Infrastructure.csproj`

### put dapper's design (ae9357a)
- Added `ISqlConnectionFactory` interface under `Data/` to abstract SQL connection creation
- Added `SqlConnectionFactory` implementation wrapping `IDbConnection`
- Registered `SqlConnectionFactory` in `DependencyInjection.cs`
- Refactored `RoomRepository` to use Dapper queries instead of EF Core — raw SQL via `IDbConnection`
- Refactored `RoomTypeRepository` to use Dapper
- Extended `Room` domain model with properties needed for Dapper mapping

## Decisions
- Introduced `ISqlConnectionFactory` as an abstraction so repositories don't depend directly on a concrete connection type — keeps repositories testable
- Dapper used only in Infrastructure layer, domain and application layers unchanged

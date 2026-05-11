# Feature History


## Summary
**Feature:** Dapper Integration
**Date:** 2026-05-12

Replaced EF Core repository implementations in RoomManagement with Dapper to give explicit control over SQL queries in the infrastructure layer. A new `ISqlConnectionFactory` abstraction was introduced so repositories remain testable without coupling to a concrete connection type. Domain and application layers were left untouched — only the infrastructure changed.

**Changes:**
- Added Dapper NuGet package to `RoomManagement.Infrastructure`
- Added `ISqlConnectionFactory` / `SqlConnectionFactory` for SQL connection abstraction
- Refactored `RoomRepository` and `RoomTypeRepository` to use Dapper raw SQL
- Extended `Room` domain model with properties required for Dapper mapping
- Registered `SqlConnectionFactory` in the DI container

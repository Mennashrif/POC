# CLAUDE.md
This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

If CLAUDE-WIP.md exists in the repo root, read it at the start of every session for current feature context.

## Project Overview

eSAP (Smart Accommodation Platform) is a proof-of-concept distributed microservices system for hotel/accommodation management. It demonstrates enterprise patterns: CQRS, Outbox Pattern, event-driven messaging, real-time notifications, and API gateway.

## Tech Stack

**Backend:** .NET 10, ASP.NET Core 10, EF Core 10, MediatR 14, Hangfire 1.8, RabbitMQ 7, Keycloak (OIDC/JWT), Redis, SQL Server, YARP 2.3, SignalR  
**Frontend:** React 18, Vite 5, @microsoft/signalr

## Services & Ports

| Service | Port | Database |
|---|---|---|
| MyGateway (API Gateway) | 7001 | — |
| Booking | 7003 | BookingDbHangFire |
| RoomManagement | 7062 | RoomManagementDb |
| Billing | 7201 | BillingDb |
| Login | 7138 | — |
| Notification | 7248 | — |
| Frontend (Vite dev) | 3000 | — |
| Keycloak | 8080 | — |

## Build & Run Commands

### Backend (any service)
```powershell
dotnet build
dotnet run --project <ServiceName>/<ServiceName>.Api
dotnet test                          # run all tests in a project
dotnet ef migrations add <Name>      # add EF migration
dotnet ef database update            # apply migrations
```

### Frontend
```powershell
cd Frontend
npm install
npm run dev      # dev server on port 3000
npm run build    # production build
```

### Infrastructure (must be running)
- **SQL Server** — all services need DB connections defined in `appsettings.json`
- **RabbitMQ** — used by Booking, RoomManagement, Billing
- **Redis** — used by MyGateway (token refresh) and Billing (permission cache)
- **Keycloak** — realm `esap` at `http://localhost:8080`
- **Hangfire Dashboard** — `/hangfire` on each service that uses it

## Architecture

### Clean Architecture Layers (per service)
Each service follows the same layered structure:
```
<Service>/
  <Service>.Domain/          # Entities, value objects, domain events, repository interfaces
  <Service>.Application/     # CQRS commands/queries (MediatR), DTOs, service interfaces
  <Service>.Infrastructure/  # EF Core DbContext, repositories, RabbitMQ consumers/publishers
  <Service>.Api/             # Minimal API endpoints, DI wiring, appsettings.json
```

### CQRS with MediatR
All business logic is in `Application` as Commands and Queries dispatched via `IMediator.Send()`. The API layer only maps HTTP requests to commands/queries. Do not add business logic to endpoints.

### Outbox Pattern
Booking and RoomManagement use an outbox for reliable event publishing:
1. A domain event is written to an `OutboxMessage` table in the same DB transaction as the business write.
2. A **Hangfire recurring job** (minutely) reads pending outbox messages and publishes them to RabbitMQ.
3. `ProcessedEvent` table prevents duplicate processing (idempotency).

When adding new domain events that must be published, follow this pattern — never publish directly from a command handler.

### RabbitMQ Topology
- **Exchange:** `booking` (topic)
- **Queues:**
  - `booking.room-events` — consumed by Booking, published by RoomManagement
  - `roommanagement.reservation-events` — consumed by RoomManagement, published by Booking
  - `billing.reservation-events` — consumed by Billing, published by Booking
- Consumer registrations are in each service's `Infrastructure` layer.

### Event Flow Example
```
Frontend → Gateway (7001) → Booking (7003)
  → [DB write + OutboxMessage in one transaction]
  → [Hangfire job publishes to RabbitMQ]
  → Billing consumes → creates Bill
  → SignalR push → Frontend real-time notification
```

### API Gateway (MyGateway)
YARP reverse proxy handles:
- JWT validation (Keycloak)
- Rate limiting: sliding window, 3 req/30s
- Token refresh via Redis
- Routing: `/rooms/*` and `/roomtypes/*` → 7062, `/bills/*` → 7201, `/reservations/*` → 7003

### Keycloak Authentication
- Authority: `http://localhost:8080/realms/esap`
- JWT Bearer is validated at the Gateway; downstream services also validate (defense-in-depth).
- Billing uses claim-based permission policies (`read:bills`, `write:bills`, `upload:files`, `download:files`) cached in Redis for 30 minutes.

### Optimistic Concurrency
Reservation and Room entities use EF Core row-version (`[Timestamp]`) for optimistic concurrency. Handle `DbUpdateConcurrencyException` where relevant.

### File Management (Billing)
- Files stored locally in `uploads/` directory
- YARA rules scan uploads for malicious content before saving
- CSV data extraction is supported

### Frontend
- Proxies `/api` → `http://localhost:5000` and `/hubs` → `https://localhost:7003` (SignalR WebSocket)
- Stores Keycloak access token in `localStorage`
- Uses SignalR with automatic reconnection for real-time reservation notifications

## Key Configuration Files
- `MyGateway/appsettings.json` — gateway routes, Keycloak, service URLs, Redis
- `Booking/Booking.Api/appsettings.json` — DB, RabbitMQ, Hangfire
- `RoomManagement/RoomManagement.API/appsettings.json` — DB, RabbitMQ
- `Billing/Billing.Api/appsettings.json` — DB, Redis, file storage path, Keycloak, RabbitMQ
- `Frontend/vite.config.js` — dev proxy targets

## Adding a New Feature (Checklist)
1. **Domain:** Add entity/value object or domain event in `.Domain`
2. **Application:** Add Command or Query + Handler in `.Application`; update repository interface if needed
3. **Infrastructure:** Implement repository method or add RabbitMQ consumer/publisher in `.Infrastructure`
4. **API:** Wire endpoint in `.Api` and register any new DI services
5. **Migration:** Run `dotnet ef migrations add <Name>` in the Infrastructure project
6. **Outbox:** If the feature publishes events, write to `OutboxMessage` inside the same transaction — never publish directly

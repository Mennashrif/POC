# WIP: Claude Code project documentation

## Goal
Add CLAUDE.md to give Claude Code full project context, and configure it to auto-load CLAUDE-WIP.md at session start for current feature context.

## Changes
- Added `CLAUDE.md` with full project architecture: service map, ports, tech stack, Clean Architecture layers, CQRS/MediatR pattern, Outbox Pattern, RabbitMQ topology, API Gateway (YARP), Keycloak auth, optimistic concurrency, file management, and feature checklist.
- Added auto-load instruction at the top of `CLAUDE.md`: if `CLAUDE-WIP.md` exists in the repo root, Claude Code reads it at session start.

## Decisions
- CLAUDE.md is checked into the repo so it is available to all contributors and CI agents automatically.
- The CLAUDE-WIP.md auto-load line is placed at the very top of CLAUDE.md so it is picked up before any other content.

## Config fixes
- Updated Keycloak `Authority` port in `Billing/Billing.Api/appsettings.json` from `8180` to `8080` to match the actual running Keycloak instance.

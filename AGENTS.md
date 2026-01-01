# Librarian-CSharp Agents Overview

The project is organized around gRPC services that play different “agent” roles. The table below maps each component to its code entry and responsibility so newcomers can jump to the right place quickly.

| Component | Project / Entry Point | Responsibilities |
| --- | --- | --- |
| **Sephirah Server** | `Librarian.Sephirah.Server` → `StartUp` / `Program` | Primary host exposing gRPC (with JSON Transcoding) and Swagger/Reflection in development. Maps both `SephirahService` and `AngelaService`; runs EF Core migrations on startup; configures multi-audience JWT auth, rate limiting, optional Consul discovery, and MassTransit transport (InMemory/RabbitMQ). |
| **Sephirah Service library** | `Librarian.Sephirah` | Core business surface: the `Tiphereth` area inside `SephirahService` handles tokens, devices, and account linking; `Binah` handles MinIO-backed uploads/downloads; `PullMetadataService` polls metadata sources; `SephirahService` coordinates and tracks Porter endpoints via `SephirahContext`. |
| **Angela Service library** | `Librarian.Angela` | Management-facing APIs co-hosted in Sephirah Server. The `Tiphereth` area handles user CRUD and token refresh plus local-admin checks; `Sentinel` manages Sentinel nodes; `StoreApp`/`Gebura` search and retrieve store metadata. |
| **Porter Server** | `Librarian.Porter.Server` → `Program` | Plugin host for metadata/account ingestion. Exposes gRPC + reflection; enables Steam/Bangumi/VNDB via `PorterConfig`; `ServicesUtil` wires third-party clients; `ConsulUtil` registers health checks when enabled. Default ports: gRPC 5000, health/HTTP1 5001. |
| **Porter Service library** | `Librarian.Porter` | Implements Porter endpoints: `Gebura` fetches/parses app info, the `Tiphereth` area handles account retrieval, `Common` exposes enable/info calls. Mapped by Porter Server. |
| **Sentinel Agent library** | `Librarian.Sentinel` | For edge Sentinel probes: `Gebura` reports sentinel state and app binaries, the `Tiphereth` area provides heartbeat and token refresh. Used by whatever process hosts the agent. |
| **Angela Blazor Server** | `Librarian.Angela.BlazorServer` | Admin UI; `appsettings.json` points both `SephirahApi` and `AngelaApi` to the backend (default https://localhost:5148). |
| **Shared & infrastructure** | `Librarian.Common`, `Librarian.ThirdParty`, `Librarian.Common.Migrations.*` | Common models/configs/utils (e.g., `SystemConfig` for MinIO, static Porters, AngelaTrustedIPs), third-party API clients, and multi-database migration assemblies (SQLite/MySQL/PostgreSQL). |

## Run & configuration quick reference
- **Sephirah settings**: `Librarian.Sephirah.Server/appsettings.json` defines DB/backend (EF), JWT audiences/expiry, MinIO buckets, static Porter entries, Consul, and MassTransit.
- **Porter settings**: `Librarian.Porter.Server/appsettings.json` toggles platforms and rate limits, Consul registration, and port bindings.
- **Auth**: Sephirah uses multi-audience JWT (Access/Refresh/Upload/Download); `AngelaAuthorizationHandler` enforces management access.
- **Messaging & discovery**: `SephirahContext` stores Porter instances discovered via Consul plus statically configured ones; MassTransit transport is selectable per config.

## Developer notes
- gRPC endpoints are mapped by their respective Server projects; `AngelaService` and `SephirahService` run in the same Sephirah Server process.
- Static Porters (`SystemConfig.StaticPorterInstances`) allow environments without Consul; tags (e.g., `platform:*`) are used when creating endpoints in `PullMetadataService`.
- Sephirah executes DB migrations at startup; Binah file flows rely on MinIO configuration. Dev mode can enable gRPC-Web and Swagger for easier debugging.

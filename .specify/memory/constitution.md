<!--
===============================================================================
SYNC IMPACT REPORT
===============================================================================
Version Change: N/A → 0.2.0 (Initial adoption)
Ratification Date: 2026-01-30
Last Amended: 2026-01-30

Modified Principles: N/A (Initial constitution)
Added Sections:
  - Core Principles (5 principles)
  - Technology Stack
  - Development Workflow
  - Governance

Templates Requiring Updates:
  - .specify/templates/plan-template.md ✅ (compatible)
  - .specify/templates/spec-template.md ✅ (compatible)
  - .specify/templates/tasks-template.md ✅ (compatible)

Follow-up TODOs: None
===============================================================================
-->

# Librarian-CSharp Constitution

## Core Principles

### I. Modular Architecture

- Every major feature MUST reside in its own project/library within the solution
- Projects follow clear separation: Common (shared), Server (host), Service (business logic)
- Cross-project dependencies MUST flow downward (Server → Service → Common)
- Third-party integrations MUST be isolated in dedicated projects (e.g., Librarian.ThirdParty)
- Each project MUST have a single, clear responsibility

### II. gRPC-First API Design

- All inter-service communication MUST use gRPC with TuiHub.Protos
- Protocol buffer definitions serve as the contract source of truth
- Services MUST implement proper gRPC reflection for development environments
- API versioning follows proto package versioning (e.g., `TuiHub.Protos.Librarian.Sephirah.V1`)
- REST endpoints are optional and MUST NOT replace gRPC contracts

### III. Test Coverage

- Unit tests MUST cover core business logic in service layers
- Integration tests MUST verify gRPC service endpoints
- Tests use xUnit and MSTest frameworks with coverlet for coverage
- New features SHOULD include corresponding test coverage
- Breaking changes MUST update affected tests before merging

### IV. Database Abstraction

- All database access MUST go through Entity Framework Core
- Database migrations MUST be isolated in provider-specific projects (MySql, PostgreSQL)
- Models MUST use nullable reference types consistently
- AutoMapper MUST be used for entity-to-DTO transformations
- Direct SQL queries SHOULD be avoided unless justified for performance

### V. Configuration & Dependency Injection

- All configuration MUST be strongly-typed using Options pattern
- Services MUST be registered via dependency injection (DI)
- Global static context usage SHOULD be minimized and eventually replaced with DI
- Configuration sections MUST fail fast with clear error messages if invalid
- Secrets MUST NOT be committed; use environment variables or secret managers

## Technology Stack

- **Runtime**: .NET 8.0 (LTS)
- **Language**: C# with nullable reference types enabled
- **ORM**: Entity Framework Core 9.x
- **Messaging**: MassTransit with RabbitMQ
- **Service Discovery**: Consul (optional)
- **Object Storage**: MinIO
- **Authentication**: JWT Bearer tokens
- **Mapping**: AutoMapper
- **ID Generation**: IdGen (Snowflake-style)
- **Testing**: xUnit, MSTest, Coverlet

## Development Workflow

- **Branch Strategy**: Feature branches merged via pull request
- **Code Style**: Follow .editorconfig and solution-level DotSettings
- **Build**: Standard `dotnet build` with solution file
- **Run**: Use `dotnet run` on Server projects or Docker containers
- **Migrations**: Generate via scripts/generate-migrations.sh
- **CI/CD**: TODO - Pipeline configuration pending

## Governance

This Constitution supersedes all other development practices for Librarian-CSharp.

- All pull requests MUST verify compliance with these principles
- Deviations from principles require explicit justification in PR description
- Constitution amendments require:
  1. Written proposal documenting the change
  2. Review by project maintainers
  3. Migration plan for affected code (if applicable)
- Version increments follow semantic versioning:
  - **MAJOR**: Backward-incompatible principle changes
  - **MINOR**: New principles or material expansions
  - **PATCH**: Clarifications and typo fixes

**Version**: 0.2.0 | **Ratified**: 2026-01-30 | **Last Amended**: 2026-01-30

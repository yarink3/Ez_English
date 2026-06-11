# 3. Backend: .NET 8 Web API (C#)

Date: 2026-06-11

## Status

Accepted

## Context

The backend needs to:

- Expose REST endpoints for parents, children, characters, lessons, and progress.
- Validate Firebase ID tokens on every authenticated request.
- Talk to Azure SQL via an ORM.
- Run cheaply on Azure App Service (Linux B1 tier is fine to start).
- Be familiar to .NET-friendly hires and integrate cleanly with Azure tooling.

Alternatives considered: Node + Express/NestJS, Python + FastAPI. All viable; .NET wins on Azure tooling, performance, and long-term maintainability for a CRUD-heavy API.

## Decision

Use **.NET 8 (LTS) Web API** with the following building blocks:

- **ASP.NET Core minimal API + controllers** (controllers for non-trivial resources, minimal endpoints for health/diagnostics).
- **Entity Framework Core 8** with the SQL Server provider for Azure SQL.
- **FirebaseAdmin SDK** (`FirebaseAdmin` NuGet) for ID token validation in custom JWT bearer middleware.
- **Serilog** for structured logging to Application Insights.
- **FluentValidation** for request validation.
- **Swagger / Swashbuckle** for API documentation (dev/staging only).
- Project layout (Clean Architecture-lite):
  - `EzEnglish.Api` — HTTP layer (controllers, middleware, DI composition).
  - `EzEnglish.Application` — use cases / services.
  - `EzEnglish.Domain` — entities, value objects, enums (Level, Age, ContentCategory).
  - `EzEnglish.Infrastructure` — EF Core DbContext, repositories, Firebase integration.

> Note: we may scaffold all four projects under a single `backend/` solution. For Phase 1 we will start with a single `EzEnglish.Api` project and split as complexity grows (YAGNI).

## Consequences

- Excellent perf and tooling on Azure.
- C# strong typing protects domain invariants (e.g., `AgeGroup` enum, `LessonLevel` value object).
- Slight ramp-up cost for contributors unfamiliar with .NET.
- We must keep an eye on Firebase Admin SDK + .NET 8 compatibility (currently fine).

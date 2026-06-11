# 4. Database: Azure SQL Database with EF Core

Date: 2026-06-11

## Status

Accepted

## Context

The domain is highly relational: parents own many children; children have one current level per category; lessons contain ordered items; progress is a many-to-many of children × lesson items with timestamps and scores. Relational integrity matters (we don't want orphaned children or progress entries pointing at deleted lessons).

We considered:

- **Azure SQL Database** — fully managed, strong relational model, native integration with .NET/EF Core, generous free tier and serverless options.
- **Azure Cosmos DB** — better for huge scale / flexible schema, but overkill and harder to model parent/child/progress relations.
- **PostgreSQL on Azure** — solid open-source alternative; we pick SQL Server purely for the tighter .NET/EF tooling.

## Decision

Use **Azure SQL Database** (serverless, Gen5) accessed via **Entity Framework Core 8** with code-first migrations.

Initial schema (subject to refinement in Phase 2):

- `Parents` (Id, FirebaseUid, Email, DisplayName, CreatedAtUtc)
- `Children` (Id, ParentId FK, DisplayName, BirthDate, CharacterId FK, CreatedAtUtc)
- `Characters` (Id, Key, DisplayNameEn, DisplayNameHe, AvatarUrl)
- `Lessons` (Id, Category, Level, TitleEn, TitleHe, OrderInLevel)
- `LessonItems` (Id, LessonId FK, Kind, PromptEn, ImageUrl, AudioUrl, ExpectedAnswer, OrderInLesson)
- `Progress` (Id, ChildId FK, LessonItemId FK, Score, AttemptedAtUtc, CompletedAtUtc)

Indexes:

- `Children.ParentId`
- `Progress.ChildId, Progress.LessonItemId` (composite, unique)
- `Lessons.Category, Lessons.Level, Lessons.OrderInLevel` (composite)

## Consequences

- Familiar to most .NET developers; great tooling (SSMS, Azure Data Studio).
- Migrations live in source control; reproducible deployments.
- Cost: serverless auto-pause keeps dev costs near zero.
- Vendor lock-in to SQL Server flavor (mostly fine; we avoid T-SQL-isms in EF queries).

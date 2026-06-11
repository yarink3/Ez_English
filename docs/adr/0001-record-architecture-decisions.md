# 1. Record architecture decisions

Date: 2026-06-11

## Status

Accepted

## Context

We are starting a brand-new project (Ez_English) — an English-learning web app for Israeli children aged 3–12. Several non-trivial choices (frontend, backend, database, auth, deployment, content model) need to be made early and remembered, because they shape every subsequent decision.

We need a lightweight, durable way to record *why* each major choice was made, not just *what* the choice was.

## Decision

We will use Architecture Decision Records (ADRs) as described by Michael Nygard.

- Each ADR lives in `docs/adr/` as a sequentially numbered Markdown file: `NNNN-short-title.md`.
- Each ADR has the sections: **Status**, **Context**, **Decision**, **Consequences**.
- Status is one of: Proposed, Accepted, Deprecated, Superseded by ADR-XXXX.
- ADRs are immutable once accepted; to change a decision, write a new ADR that supersedes the old one.

## Consequences

- New contributors can see *why* the architecture looks the way it does, not just *what* it is.
- Decisions are reviewable in PRs like any other artifact.
- Slight overhead per major decision (~10 min to write an ADR), but pays back the first time someone asks "why did we pick X?".

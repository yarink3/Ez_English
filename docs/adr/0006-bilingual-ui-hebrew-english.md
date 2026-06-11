# 6. Bilingual UI: Hebrew chrome + English learning content

Date: 2026-06-11

## Status

Accepted

## Context

Our users are Israeli children learning English, and their Hebrew-speaking parents.

- **Parents** (dashboard, account, child management) read Hebrew comfortably and benefit from a Hebrew UI.
- **Children** (ages 3–12) are learning English; their *learning content* (words, prompts, audio, mascot encouragement) should be primarily English, possibly with Hebrew hints where pedagogically useful.

This means we have two distinct layers:

1. **UI chrome strings** (buttons, menu items, error messages) — Hebrew (and English fallback).
2. **Learning content** (lesson titles, prompts, audio) — English with optional Hebrew translation field for hints/glossing.

## Decision

- Use **i18next + react-i18next** for chrome-string localization.
  - Default locale: `he`. Fallback locale: `en`.
  - Translation files under `frontend/src/i18n/locales/{he,en}/common.json`.
- Top-level `<html dir="rtl" lang="he">` set dynamically based on current locale.
- Use Tailwind logical properties (`ps-`/`pe-`/`ms-`/`me-`) and `rtl:` / `ltr:` variants. Avoid hard-coded `left`/`right`.
- For **learning content**, store both `*En` and `*He` fields in the database (e.g., `Lessons.TitleEn`, `Lessons.TitleHe`). The lesson player shows English prominently; Hebrew is shown as a smaller hint, toggleable per child age group (e.g., shown by default for ages 3–6, hidden for 9+).
- All English learning content text is wrapped in `<span dir="ltr">` to render correctly inside an RTL document.

## Consequences

- Parents get a fully native Hebrew dashboard.
- Children get authentic English immersion with age-appropriate scaffolding.
- All developers must consistently use logical CSS properties — we'll add an ESLint rule (`eslint-plugin-rtl-friendly` or a custom check) in a later phase.
- Translation file maintenance is dual-track (he + en). Acceptable.

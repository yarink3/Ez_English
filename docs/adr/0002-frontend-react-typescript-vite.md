# 2. Frontend: React + TypeScript + Vite

Date: 2026-06-11

## Status

Accepted

## Context

The app targets Israeli children aged 3–12 and their parents. The UI must be:

- Highly interactive (drag-and-drop, animated mascot characters, audio playback)
- Bilingual: Hebrew chrome (parent dashboard, menus, buttons) + English learning content; Hebrew implies RTL layout support
- Easy to deploy on Azure
- Approachable for a small team; rich ecosystem of UI / game / animation libraries

We considered:

- **React + TypeScript (Vite)** — mature, huge ecosystem, first-class TS, Vite gives fast dev loop. Deploys cleanly to Azure Static Web Apps.
- **Next.js** — overkill for a primarily client-side, kid-facing SPA. SSR adds complexity we don't need.
- **React Native / Flutter** — mobile-first; user explicitly chose web.

## Decision

Use **React 18 + TypeScript + Vite** for the frontend.

Supporting libraries:

- **Tailwind CSS** for styling, with RTL utilities (`rtl:` variants) for Hebrew layout.
- **react-router-dom** for routing.
- **i18next + react-i18next** for Hebrew/English localization of UI chrome.
- **@dnd-kit/core** for drag-and-drop interactions (modern, accessible, touch-friendly — important for tablets).
- **Framer Motion** for character animations.
- **TanStack Query** for backend data fetching/caching.
- **Firebase JS SDK** for authentication (see ADR-0005).

## Consequences

- Fast iteration via Vite HMR.
- Native TS catches errors early — important for a kids' app that must not crash.
- We accept the SPA tradeoff (no SSR/SEO); acceptable because the app is behind login and not content-discoverable.
- Tailwind's `dir="rtl"` story is good but requires discipline (use logical `start/end` properties instead of `left/right`).

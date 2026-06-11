# 9. Learning content & game model

Date: 2026-06-11

## Status

Accepted

## Context

The app needs to teach colors, animals, family members, letters, and more — using drag-and-drop games and other interactive formats. We need a content model flexible enough to add new categories and game types without schema changes, but structured enough to compute progress and recommend next lessons.

## Decision

### Concepts

- **Category** — broad subject area: `Colors`, `Animals`, `Family`, `Letters`, `Numbers`, … (enum).
- **Level** — see ADR-0008 (Pre-A1 → B1).
- **Lesson** — a unit of teaching belonging to one category and one level (`Colors / Pre-A1 / Lesson 3: Red, Blue, Yellow`).
- **LessonItem** — an individual interaction within a lesson; has a `Kind`:
  - `DragMatch` — drag English label onto matching picture.
  - `TapPick` — listen to audio, tap the matching picture.
  - `LetterTrace` — trace a letter with finger / mouse.
  - `Memory` — flip-card pair matching.
  - `Sort` — drag items into category bins.
- **Progress** — per `(ChildId, LessonItemId)`: score 0–100, first-attempt timestamp, completion timestamp, attempt count.

### Reusable game engine

Rather than hand-coding each lesson, lessons are **data-driven JSON** stored in `LessonItems`. The frontend has a small set of React components (one per `Kind`) that render any lesson item conforming to that kind's schema. Adding a new lesson = inserting rows, not writing code.

Example `DragMatch` item payload:

```json
{
  "kind": "DragMatch",
  "promptEn": "Match the color to its name",
  "promptHe": "התאימו את הצבע לשם שלו",
  "pairs": [
    { "image": "/img/colors/red.svg",    "label": "red"    },
    { "image": "/img/colors/blue.svg",   "label": "blue"   },
    { "image": "/img/colors/yellow.svg", "label": "yellow" }
  ],
  "audioPromptUrl": "/audio/colors/match-prompt.mp3"
}
```

### Mascot character

Each child picks one of N mascots (e.g., Lion, Bunny, Owl, Robot). The mascot appears in the corner of every lesson and reacts (animations + audio) to correct/incorrect answers. Mascot personality strings (encouragement, hints) are part of the `Characters` table with `*En`/`*He` variants.

### Recommendation

For v1, the "next lesson" is simply the next `OrderInLevel` for the child's current `(Category, Level)`. Once a level's lessons are all ≥80% complete, the child auto-promotes. Smarter spaced-repetition is a Phase 4+ concern.

## Consequences

- Authors and educators can add content without code changes (long-term: a content authoring tool).
- Frontend stays small: ~5 game components cover most lessons.
- Schema is stable; new game kinds = new enum value + new React component, no migration.
- We trade some up-front design effort for big downstream velocity.

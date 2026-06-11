# 8. Parent / Child account model

Date: 2026-06-11

## Status

Accepted

## Context

Children aged 3–12 should not be required to create their own email-based accounts (COPPA-friendly, also impractical for a 4-year-old). Parents must be able to manage one or more child profiles under a single login, and each child should have:

- A name and birth date (drives age group → starting level).
- A chosen **mascot character** that escorts them through lessons.
- An independent **level per learning category** (Colors, Animals, Family, Letters, …) — a child may be advanced in Letters but a beginner in Animals.
- Per-child **progress records**.

## Decision

### Identity boundary

- **Parent** = the only authenticated principal (Firebase ID token).
- **Child** = a domain entity owned by a parent; no Firebase identity.

### Age → starting level mapping

| Age range | Starting level | Notes |
|-----------|----------------|-------|
| 3–4 | `Pre-A1 / Pre-Reader` | Audio-first, picture-based; minimal text |
| 5–6 | `A0 / Early Learner` | Single letters, simple words, lots of repetition |
| 7–8 | `A1 / Beginner` | Short words, basic sentence structure, more reading |
| 9–10 | `A2 / Elementary` | Longer prompts, listening comprehension |
| 11–12 | `B1 / Pre-Intermediate` | Reading comprehension, basic grammar games |

A child can be promoted (or demoted) per category independently, based on progress and parent override.

### Per-category level

```
Child (1) ──── (N) ChildCategoryLevel { Category, Level }
```

`ChildCategoryLevel` is a small table with `(ChildId, Category)` unique. Initialized at child creation from the age → level mapping above.

### Optional child PIN

Each child *may* have a 4-digit PIN (stored as a bcrypt/Argon2 hash). PIN is only used as a soft barrier so siblings on a shared tablet pick the correct profile — it is **not** an authentication mechanism in the security sense.

### Authorization rule

Every API endpoint that accepts a `childId` MUST verify `Children.ParentId == AuthenticatedParent.Id`. Centralize this check in an authorization handler (`ChildOwnedByCurrentParentRequirement`) so it can't be forgotten in a new controller.

## Consequences

- Simple, clean ownership model.
- Easy to scope all queries by `ParentId`.
- A child can never log in or interact with the API without their parent's authenticated device.
- If parents share devices across families later (e.g., grandparents), we'll need a more nuanced sharing model — out of scope for v1.

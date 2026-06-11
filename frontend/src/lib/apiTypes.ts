/** Mirrors backend enums in EzEnglish.Api.Domain.Enums. Keep in sync. */
export const Category = {
  Colors:  1,
  Animals: 2,
  Family:  3,
  Letters: 4,
  Numbers: 5,
} as const
export type Category = (typeof Category)[keyof typeof Category]

export const Level = {
  PreA1: 1,
  A0:    2,
  A1:    3,
  A2:    4,
  B1:    5,
} as const
export type Level = (typeof Level)[keyof typeof Level]

const LEVEL_LABELS: Record<Level, string> = {
  1: 'PreA1',
  2: 'A0',
  3: 'A1',
  4: 'A2',
  5: 'B1',
}
export const levelLabel = (lvl: Level): string => LEVEL_LABELS[lvl] ?? String(lvl)

export type CharacterDto = {
  id: number
  key: string
  displayNameEn: string
  displayNameHe: string
  avatarUrl: string
}

export type ChildCategoryLevelDto = {
  category: Category
  level: Level
}

export type ChildDto = {
  id: number
  displayName: string
  birthDate: string         // ISO "YYYY-MM-DD"
  characterId: number
  categoryLevels: ChildCategoryLevelDto[]
}

export type ParentDto = {
  id: number
  email: string
  displayName: string | null
}

export type MeResponse = {
  parent: ParentDto
  children: ChildDto[]
}

export type CreateChildRequest = {
  displayName: string
  birthDate: string         // ISO "YYYY-MM-DD"
  characterId: number
  pin?: string | null
}

/** Mirrors backend EzEnglish.Api.Domain.Enums.LessonItemKind. */
export const LessonItemKind = {
  DragMatch:   1,
  TapPick:     2,
  LetterTrace: 3,
  Memory:      4,
  Sort:        5,
} as const
export type LessonItemKind = (typeof LessonItemKind)[keyof typeof LessonItemKind]

export type LessonSummaryDto = {
  id: number
  category: Category
  level: Level
  titleEn: string
  titleHe: string
  orderInLevel: number
  itemCount: number
}

/** Shape of payload for a DragMatch item — matches backend seed JSON. */
export type DragMatchPayload = {
  pairs: { image: string; label: string }[]
  audioPromptUrl?: string | null
}

export type LessonItemDto = {
  id: number
  kind: LessonItemKind
  orderInLesson: number
  promptEn: string
  promptHe: string | null
  payload: unknown
}

export type LessonDetailDto = {
  id: number
  category: Category
  level: Level
  titleEn: string
  titleHe: string
  items: LessonItemDto[]
}

export type SubmitProgressRequest = {
  lessonItemId: number
  score: number          // 0–100
}

export type ProgressDto = {
  lessonItemId: number
  score: number
  attemptCount: number
  firstAttemptedAtUtc: string
  lastAttemptedAtUtc: string
  completedAtUtc: string | null
}

/**
 * Lightweight per-child progress tracker for unlocking categories.
 *
 * The backend already records per-item progress; this helper just gives the
 * UI a fast way to know "did the child complete the lesson for category X?"
 * without round-tripping every category's lesson list. Stored in localStorage
 * so it survives reloads on the same device.
 */

import { Category } from './apiTypes'

const STORAGE_KEY = 'ezEnglish.completedCategories.v1'

/** Stage order — index N is unlocked iff stage N-1 is complete. */
export const CATEGORY_ORDER: Category[] = [
  Category.Colors,
  Category.Animals,
  Category.Family,
  Category.Letters,
  Category.Numbers,
]

type Store = Record<string, Record<string, true>>

function read(): Store {
  if (typeof window === 'undefined') return {}
  try {
    const raw = window.localStorage.getItem(STORAGE_KEY)
    if (!raw) return {}
    const parsed = JSON.parse(raw) as unknown
    return (parsed && typeof parsed === 'object') ? (parsed as Store) : {}
  } catch {
    return {}
  }
}

function write(store: Store): void {
  if (typeof window === 'undefined') return
  try {
    window.localStorage.setItem(STORAGE_KEY, JSON.stringify(store))
    window.dispatchEvent(new CustomEvent('ezenglish:progress-updated'))
  } catch {
    /* localStorage may be full or disabled — non-fatal */
  }
}

export function markCategoryComplete(childId: number, category: Category): void {
  const store = read()
  const key = String(childId)
  const childMap = store[key] ?? {}
  childMap[String(category)] = true
  store[key] = childMap
  write(store)
}

export function isCategoryComplete(childId: number, category: Category): boolean {
  return !!read()[String(childId)]?.[String(category)]
}

/** A category is unlocked if it's the first one, or the previous one is complete. */
export function isCategoryUnlocked(childId: number, category: Category): boolean {
  const idx = CATEGORY_ORDER.indexOf(category)
  if (idx <= 0) return true
  const prev = CATEGORY_ORDER[idx - 1]
  return isCategoryComplete(childId, prev)
}

/** Returns the next stage after `category`, or null if it's already the last one. */
export function nextCategory(category: Category): Category | null {
  const idx = CATEGORY_ORDER.indexOf(category)
  if (idx < 0 || idx >= CATEGORY_ORDER.length - 1) return null
  return CATEGORY_ORDER[idx + 1]
}

/** Shared result shape returned by every lesson-item game component. */
export type GameResult = {
  /** Number of mistakes the kid made before completing the item. */
  mistakes: number
  /** Score 0–100, derived per-game (no mistakes = 100). */
  score: number
}

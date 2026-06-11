/**
 * Tiny wrapper over the Web Speech Synthesis API for "tap the word, hear the word".
 * Prefers an English female voice if available; falls back gracefully otherwise.
 *
 * NOTE: Browsers populate the voice list asynchronously. We listen for
 * `voiceschanged` and cache the chosen voice once it's available.
 */

let cachedVoice: SpeechSynthesisVoice | null = null

const FEMALE_NAME_PATTERNS: RegExp[] = [
  /female/i,
  /samantha/i,
  /victoria/i,
  /karen/i,
  /tessa/i,
  /susan/i,
  /allison/i,
  /zira/i,
  /serena/i,
  /moira/i,
  /fiona/i,
  /^Google US English$/,
  /^Microsoft Aria/i,
  /^Microsoft Jenny/i,
]

function pickVoice(): SpeechSynthesisVoice | null {
  if (typeof window === 'undefined' || !('speechSynthesis' in window)) return null
  const voices = window.speechSynthesis.getVoices()
  if (!voices.length) return null
  const english = voices.filter((v) => v.lang.toLowerCase().startsWith('en'))
  const candidates = english.length ? english : voices
  for (const pattern of FEMALE_NAME_PATTERNS) {
    const match = candidates.find((v) => pattern.test(v.name))
    if (match) return match
  }
  // Last resort: prefer a non-male default English voice, otherwise the first one.
  return candidates[0] ?? null
}

if (typeof window !== 'undefined' && 'speechSynthesis' in window) {
  // Touch getVoices() once to nudge browsers that load voices lazily.
  window.speechSynthesis.getVoices()
  window.speechSynthesis.addEventListener('voiceschanged', () => {
    cachedVoice = pickVoice()
  })
}

export function speak(text: string, lang: string = 'en-US'): void {
  if (typeof window === 'undefined' || !('speechSynthesis' in window)) return
  const trimmed = text?.trim()
  if (!trimmed) return
  try {
    window.speechSynthesis.cancel()
    const utter = new SpeechSynthesisUtterance(trimmed)
    utter.lang = lang
    utter.rate = 0.9
    utter.pitch = 1.1
    const voice = cachedVoice ?? (cachedVoice = pickVoice())
    if (voice) utter.voice = voice
    window.speechSynthesis.speak(utter)
  } catch {
    /* swallow — TTS is a non-essential nicety */
  }
}

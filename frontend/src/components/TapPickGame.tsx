/**
 * Tap-the-right-one game (a.k.a. multiple choice with images).
 * The kid hears/sees a target word and taps the matching picture from a row
 * of candidates. Used everywhere: "Find the RED one", "Find the CAT", etc.
 */
import { useEffect, useMemo, useState } from 'react'
import { motion } from 'framer-motion'
import { useTranslation } from 'react-i18next'
import type { TapPickPayload } from '../lib/apiTypes'
import type { GameResult } from './gameTypes'
import ItemImage from './ItemImage'
import { speak } from '../lib/tts'

type Props = {
  payload: TapPickPayload
  onComplete: (result: GameResult) => void
  onAttempt?: (correct: boolean) => void
}

function shuffle<T>(arr: T[]): T[] {
  const a = arr.slice()
  for (let i = a.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1))
    ;[a[i], a[j]] = [a[j], a[i]]
  }
  return a
}

export default function TapPickGame({ payload, onComplete, onAttempt }: Props) {
  const { t } = useTranslation()
  const choices = useMemo(() => shuffle(payload.choices), [payload])
  const [mistakes, setMistakes] = useState(0)
  const [picked, setPicked] = useState<string | null>(null)
  const [shake, setShake] = useState<string | null>(null)
  const [done, setDone] = useState(false)

  // Speak the target word once when the round starts.
  useEffect(() => {
    const id = window.setTimeout(() => speak(payload.correctLabel), 250)
    return () => window.clearTimeout(id)
  }, [payload.correctLabel])

  const handlePick = (label: string) => {
    if (done) return
    if (label === payload.correctLabel) {
      setPicked(label)
      setDone(true)
      onAttempt?.(true)
      speak(label)
      const score = Math.max(0, 100 - mistakes * 25)
      window.setTimeout(() => onComplete({ mistakes, score }), 700)
    } else {
      setMistakes((m) => m + 1)
      setShake(label)
      onAttempt?.(false)
      window.setTimeout(() => setShake((s) => (s === label ? null : s)), 500)
    }
  }

  return (
    <div className="flex flex-col items-center gap-6">
      <button
        type="button"
        onClick={() => speak(payload.correctLabel)}
        className="rounded-kid bg-amber-300 px-6 py-3 text-2xl font-bold text-slate-900 shadow-kid hover:bg-amber-400"
        aria-label={t('play.replayWord', { defaultValue: 'Hear again' })}
      >
        🔊 {payload.correctLabel}
      </button>

      <div className="grid w-full grid-cols-2 gap-4 sm:grid-cols-3">
        {choices.map((c) => {
          const isPicked = picked === c.label
          return (
            <motion.button
              key={c.label}
              type="button"
              onClick={() => handlePick(c.label)}
              animate={shake === c.label ? { x: [-8, 8, -8, 8, 0] } : { x: 0 }}
              transition={{ duration: 0.4 }}
              className={
                'flex flex-col items-center gap-2 rounded-kid bg-white p-4 shadow-kid transition ' +
                (isPicked
                  ? 'border-4 border-emerald-400 bg-emerald-50'
                  : 'border-4 border-transparent hover:scale-105')
              }
            >
              <div className="flex h-24 w-24 items-center justify-center">
                <ItemImage image={c.image} alt={c.label} className="h-20 w-20" />
              </div>
              <span className="text-sm font-semibold text-slate-700">{c.label}</span>
            </motion.button>
          )
        })}
      </div>
    </div>
  )
}

/**
 * Card-flip Memory game. Each pair generates two cards (one image card, one
 * label card) and the kid taps two at a time to find matches. Inspired by
 * Khan Academy Kids' classic Memory activity.
 */
import { useEffect, useMemo, useState } from 'react'
import { motion } from 'framer-motion'
import type { MemoryPayload } from '../lib/apiTypes'
import type { GameResult } from './gameTypes'
import ItemImage from './ItemImage'
import { speak } from '../lib/tts'

type Card = {
  id: string
  pairKey: string
  variant: 'image' | 'label'
  image: string
  label: string
}

type Props = {
  payload: MemoryPayload
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

export default function MemoryGame({ payload, onComplete, onAttempt }: Props) {
  const cards: Card[] = useMemo(() => {
    const built: Card[] = []
    for (const p of payload.pairs) {
      built.push({ id: `${p.label}-img`,   pairKey: p.label, variant: 'image', image: p.image, label: p.label })
      built.push({ id: `${p.label}-label`, pairKey: p.label, variant: 'label', image: p.image, label: p.label })
    }
    return shuffle(built)
  }, [payload])

  const [flipped, setFlipped] = useState<string[]>([])
  const [matched, setMatched] = useState<Record<string, true>>({})
  const [mistakes, setMistakes] = useState(0)
  const [locked, setLocked] = useState(false)
  const totalPairs = payload.pairs.length

  useEffect(() => {
    if (Object.keys(matched).length === totalPairs && totalPairs > 0) {
      const score = Math.max(0, 100 - mistakes * 10)
      const id = window.setTimeout(() => onComplete({ mistakes, score }), 600)
      return () => window.clearTimeout(id)
    }
  }, [matched, totalPairs, mistakes, onComplete])

  const handleFlip = (card: Card) => {
    if (locked) return
    if (matched[card.pairKey]) return
    if (flipped.includes(card.id)) return
    if (card.variant === 'label') speak(card.label)

    const next = [...flipped, card.id]
    setFlipped(next)
    if (next.length < 2) return

    setLocked(true)
    const [aId, bId] = next
    const a = cards.find((c) => c.id === aId)!
    const b = cards.find((c) => c.id === bId)!
    if (a.pairKey === b.pairKey) {
      onAttempt?.(true)
      speak(a.label)
      window.setTimeout(() => {
        setMatched((m) => ({ ...m, [a.pairKey]: true }))
        setFlipped([])
        setLocked(false)
      }, 600)
    } else {
      setMistakes((m) => m + 1)
      onAttempt?.(false)
      window.setTimeout(() => {
        setFlipped([])
        setLocked(false)
      }, 900)
    }
  }

  return (
    <div
      className={
        'grid gap-3 ' +
        (cards.length <= 6 ? 'grid-cols-3' : cards.length <= 8 ? 'grid-cols-4' : 'grid-cols-4 sm:grid-cols-5')
      }
    >
      {cards.map((card) => {
        const isFaceUp = flipped.includes(card.id) || !!matched[card.pairKey]
        const isMatched = !!matched[card.pairKey]
        return (
          <motion.button
            key={card.id}
            type="button"
            onClick={() => handleFlip(card)}
            whileTap={{ scale: 0.95 }}
            disabled={isMatched}
            className={
              'relative flex h-24 w-full items-center justify-center rounded-kid border-4 text-center shadow-kid transition ' +
              (isMatched
                ? 'border-emerald-400 bg-emerald-50'
                : isFaceUp
                  ? 'border-brand-400 bg-white'
                  : 'border-brand-500 bg-brand-500 text-white hover:scale-[1.03]')
            }
          >
            {isFaceUp ? (
              card.variant === 'image' ? (
                <ItemImage image={card.image} alt={card.label} className="h-16 w-16" emojiSize="3rem" />
              ) : (
                <span className="px-2 text-base font-bold text-slate-800">{card.label}</span>
              )
            ) : (
              <span className="text-3xl font-black">?</span>
            )}
          </motion.button>
        )
      })}
    </div>
  )
}

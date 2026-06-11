import { useEffect, useMemo, useRef, useState } from 'react'
import {
  DndContext,
  DragOverlay,
  PointerSensor,
  KeyboardSensor,
  useSensor,
  useSensors,
  useDraggable,
  useDroppable,
  type DragEndEvent,
} from '@dnd-kit/core'
import { motion } from 'framer-motion'
import { useTranslation } from 'react-i18next'
import type { DragMatchPayload } from '../lib/apiTypes'
import type { GameResult } from './gameTypes'
import ItemImage from './ItemImage'
import { speak } from '../lib/tts'

export type DragMatchResult = GameResult

type Props = {
  payload: DragMatchPayload
  /** Called when ALL pairs have been correctly matched. */
  onComplete: (result: DragMatchResult) => void
  /** Called for every drop, so parent can flash the mascot. */
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

export default function DragMatchGame({ payload, onComplete, onAttempt }: Props) {
  const { t } = useTranslation()
  // distance: 8 means a quick tap (no movement) fires onClick instead of starting a drag,
  // so kids can tap the word to hear it.
  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 8 } }),
    useSensor(KeyboardSensor),
  )
  const labels = useMemo(() => shuffle(payload.pairs.map((p) => p.label)), [payload])
  const [placed, setPlaced] = useState<Record<string, true>>({})
  const [mistakes, setMistakes] = useState(0)
  const [shake, setShake] = useState<string | null>(null)
  const [activeLabel, setActiveLabel] = useState<string | null>(null)
  const total = payload.pairs.length
  const done = Object.keys(placed).length
  const completedRef = useRef(false)

  useEffect(() => {
    if (done === total && total > 0 && !completedRef.current) {
      completedRef.current = true
      const score = Math.max(0, 100 - mistakes * 20)
      onComplete({ mistakes, score })
    }
  }, [done, total, mistakes, onComplete])

  const handleDragEnd = (e: DragEndEvent) => {
    setActiveLabel(null)
    const labelId = String(e.active.id)
    const overId = e.over ? String(e.over.id) : null
    if (!overId) return
    if (labelId === overId) {
      setPlaced((prev) => ({ ...prev, [labelId]: true }))
      onAttempt?.(true)
      speak(labelId)
    } else {
      setMistakes((m) => m + 1)
      setShake(labelId)
      onAttempt?.(false)
      window.setTimeout(() => setShake(null), 500)
    }
  }

  return (
    <DndContext
      sensors={sensors}
      onDragStart={(e) => setActiveLabel(String(e.active.id))}
      onDragCancel={() => setActiveLabel(null)}
      onDragEnd={handleDragEnd}
    >
      <div className="grid grid-cols-3 gap-4 sm:grid-cols-3">
        {payload.pairs.map((p) => (
          <ImageSlot key={p.label} pair={p} matched={!!placed[p.label]} />
        ))}
      </div>

      <div className="mt-8 flex flex-wrap justify-center gap-3">
        {labels.map((label) =>
          placed[label] ? null : (
            <DraggableLabel
              key={label}
              id={label}
              shaking={shake === label}
              hidden={activeLabel === label}
              onTap={() => speak(label)}
            >
              {label}
            </DraggableLabel>
          ),
        )}
      </div>

      <p className="mt-4 text-center text-sm text-slate-500">
        {t('play.progress', { done, total, defaultValue: `${done} / ${total}` })}
      </p>

      {/* DragOverlay: renders the dragged chip in a portal at the pointer position
          so it always follows the cursor / touch and isn't clipped by parents. */}
      <DragOverlay dropAnimation={null}>
        {activeLabel ? (
          <button
            type="button"
            className="cursor-grabbing rounded-kid bg-amber-300 px-6 py-3 text-lg font-bold text-slate-900 shadow-2xl ring-4 ring-amber-200"
          >
            {activeLabel}
          </button>
        ) : null}
      </DragOverlay>
    </DndContext>
  )
}

function ImageSlot({ pair, matched }: { pair: { image: string; label: string }; matched: boolean }) {
  const { isOver, setNodeRef } = useDroppable({ id: pair.label })
  return (
    <div className="flex flex-col items-center">
      <button
        type="button"
        ref={setNodeRef as unknown as React.Ref<HTMLButtonElement>}
        onClick={() => matched && speak(pair.label)}
        aria-label={pair.label}
        className={
          'flex h-32 w-32 items-center justify-center rounded-kid border-4 bg-white transition ' +
          (matched
            ? 'cursor-pointer border-emerald-400 bg-emerald-50 hover:scale-105'
            : isOver
              ? 'border-brand-500 bg-brand-50'
              : 'border-dashed border-slate-300')
        }
      >
        <ItemImage image={pair.image} alt={pair.label} className="h-24 w-24 pointer-events-none" />
      </button>
      {matched && (
        <motion.button
          type="button"
          onClick={() => speak(pair.label)}
          initial={{ scale: 0.5, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          className="mt-2 rounded-full bg-emerald-500 px-3 py-1 text-sm font-bold text-white hover:bg-emerald-600"
        >
          🔊 {pair.label}
        </motion.button>
      )}
    </div>
  )
}

function DraggableLabel({
  id,
  shaking,
  hidden,
  onTap,
  children,
}: {
  id: string
  shaking: boolean
  hidden: boolean
  onTap: () => void
  children: React.ReactNode
}) {
  const { attributes, listeners, setNodeRef, isDragging } = useDraggable({ id })
  // We deliberately do NOT apply useDraggable.transform here — the DragOverlay
  // renders a clone that follows the pointer instead. That way the original
  // chip stays in place, framer-motion can own its transform for the shake
  // animation, and we avoid fighting dnd-kit for the transform property.
  return (
    <motion.button
      type="button"
      ref={setNodeRef}
      onClick={onTap}
      animate={shaking ? { x: [-8, 8, -8, 8, 0] } : { x: 0 }}
      transition={{ duration: 0.45 }}
      style={{
        touchAction: 'none',
        visibility: hidden || isDragging ? 'hidden' : 'visible',
      }}
      {...listeners}
      {...attributes}
      className="cursor-grab rounded-kid bg-amber-300 px-6 py-3 text-lg font-bold text-slate-900 shadow-kid hover:bg-amber-400 active:cursor-grabbing"
    >
      {children}
    </motion.button>
  )
}

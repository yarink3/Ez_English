import { useEffect, useMemo, useState } from 'react'
import {
  DndContext,
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

export type DragMatchResult = {
  /** Number of mistakes made before completing. */
  mistakes: number
  /** Score 0–100 (100 = no mistakes, 20-point penalty per mistake, floor 0). */
  score: number
}

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
  const sensors = useSensors(useSensor(PointerSensor), useSensor(KeyboardSensor))
  const labels = useMemo(() => shuffle(payload.pairs.map((p) => p.label)), [payload])
  const [placed, setPlaced] = useState<Record<string, true>>({})
  const [mistakes, setMistakes] = useState(0)
  const [shake, setShake] = useState<string | null>(null)
  const total = payload.pairs.length
  const done = Object.keys(placed).length

  useEffect(() => {
    if (done === total) {
      const score = Math.max(0, 100 - mistakes * 20)
      onComplete({ mistakes, score })
    }
  }, [done, total, mistakes, onComplete])

  const handleDragEnd = (e: DragEndEvent) => {
    const labelId = String(e.active.id)
    const overId = e.over ? String(e.over.id) : null
    if (!overId) return
    if (labelId === overId) {
      setPlaced((prev) => ({ ...prev, [labelId]: true }))
      onAttempt?.(true)
    } else {
      setMistakes((m) => m + 1)
      setShake(labelId)
      onAttempt?.(false)
      window.setTimeout(() => setShake(null), 500)
    }
  }

  return (
    <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
      <div className="grid grid-cols-3 gap-4 sm:grid-cols-3">
        {payload.pairs.map((p) => (
          <ImageSlot key={p.label} pair={p} matched={!!placed[p.label]} />
        ))}
      </div>

      <div className="mt-8 flex flex-wrap justify-center gap-3">
        {labels.map((label) =>
          placed[label] ? null : (
            <DraggableLabel key={label} id={label} shaking={shake === label}>
              {label}
            </DraggableLabel>
          ),
        )}
      </div>

      <p className="mt-4 text-center text-sm text-slate-500">
        {t('play.progress', { done, total })}
      </p>
    </DndContext>
  )
}

function ImageSlot({ pair, matched }: { pair: { image: string; label: string }; matched: boolean }) {
  const { isOver, setNodeRef } = useDroppable({ id: pair.label })
  return (
    <div className="flex flex-col items-center">
      <div
        ref={setNodeRef}
        className={
          'flex h-32 w-32 items-center justify-center rounded-kid border-4 bg-white transition ' +
          (matched
            ? 'border-emerald-400 bg-emerald-50'
            : isOver
              ? 'border-brand-500 bg-brand-50'
              : 'border-dashed border-slate-300')
        }
      >
        <img src={pair.image} alt="" className="h-24 w-24" />
      </div>
      {matched && (
        <motion.span
          initial={{ scale: 0.5, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          className="mt-2 rounded-full bg-emerald-500 px-3 py-1 text-sm font-bold text-white"
        >
          {pair.label}
        </motion.span>
      )}
    </div>
  )
}

function DraggableLabel({
  id,
  shaking,
  children,
}: {
  id: string
  shaking: boolean
  children: React.ReactNode
}) {
  const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({ id })
  const style: React.CSSProperties = {
    transform: transform
      ? `translate3d(${transform.x}px, ${transform.y}px, 0)`
      : undefined,
    opacity: isDragging ? 0.75 : 1,
    touchAction: 'none',
  }
  return (
    <motion.button
      ref={setNodeRef}
      style={style}
      animate={shaking ? { x: [-8, 8, -8, 8, 0] } : { x: 0 }}
      transition={{ duration: 0.45 }}
      {...listeners}
      {...attributes}
      className="cursor-grab rounded-kid bg-amber-300 px-6 py-3 text-lg font-bold text-slate-900 shadow-kid hover:bg-amber-400 active:cursor-grabbing"
    >
      {children}
    </motion.button>
  )
}

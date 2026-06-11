/**
 * Sort-into-bins game. The kid drags items (e.g. animals) into the correct
 * bucket (e.g. 🚜 Farm / 🌳 Wild). Mirrors Khan Academy Kids' "Animal
 * Categories" / "Color Sorting" activities.
 */
import { useEffect, useMemo, useState } from 'react'
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
import type { SortPayload } from '../lib/apiTypes'
import type { GameResult } from './gameTypes'
import ItemImage from './ItemImage'
import { speak } from '../lib/tts'

type Props = {
  payload: SortPayload
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

export default function SortGame({ payload, onComplete, onAttempt }: Props) {
  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 8 } }),
    useSensor(KeyboardSensor),
  )

  const items = useMemo(() => shuffle(payload.items), [payload])
  const [placed, setPlaced] = useState<Record<string, string>>({})
  const [shake, setShake] = useState<string | null>(null)
  const [mistakes, setMistakes] = useState(0)
  const [activeItem, setActiveItem] = useState<string | null>(null)

  const total = items.length
  const done = Object.keys(placed).length

  useEffect(() => {
    if (done === total && total > 0) {
      const score = Math.max(0, 100 - mistakes * 20)
      const id = window.setTimeout(() => onComplete({ mistakes, score }), 500)
      return () => window.clearTimeout(id)
    }
  }, [done, total, mistakes, onComplete])

  const handleDragEnd = (e: DragEndEvent) => {
    setActiveItem(null)
    const itemLabel = String(e.active.id)
    const overBinId = e.over ? String(e.over.id) : null
    if (!overBinId) return
    const item = items.find((i) => i.label === itemLabel)
    if (!item) return
    if (item.binId === overBinId) {
      setPlaced((p) => ({ ...p, [itemLabel]: overBinId }))
      onAttempt?.(true)
      speak(itemLabel)
    } else {
      setMistakes((m) => m + 1)
      setShake(itemLabel)
      onAttempt?.(false)
      window.setTimeout(() => setShake((s) => (s === itemLabel ? null : s)), 500)
    }
  }

  return (
    <DndContext
      sensors={sensors}
      onDragStart={(e) => setActiveItem(String(e.active.id))}
      onDragCancel={() => setActiveItem(null)}
      onDragEnd={handleDragEnd}
    >
      <div
        className={
          'grid gap-4 ' +
          (payload.bins.length === 2 ? 'grid-cols-2' : 'grid-cols-2 sm:grid-cols-3')
        }
      >
        {payload.bins.map((bin) => (
          <Bin key={bin.id} bin={bin} placedLabels={Object.keys(placed).filter((l) => placed[l] === bin.id)} />
        ))}
      </div>

      <div className="mt-8 flex flex-wrap justify-center gap-3">
        {items.map((item) =>
          placed[item.label] ? null : (
            <DraggableItem
              key={item.label}
              id={item.label}
              shaking={shake === item.label}
              hidden={activeItem === item.label}
              image={item.image}
              onTap={() => speak(item.label)}
            />
          ),
        )}
      </div>

      <DragOverlay dropAnimation={null}>
        {activeItem ? (
          <div className="flex h-20 w-20 cursor-grabbing items-center justify-center rounded-kid bg-white shadow-2xl ring-4 ring-amber-200">
            <ItemImage
              image={items.find((i) => i.label === activeItem)?.image ?? ''}
              alt={activeItem}
              className="h-16 w-16"
              emojiSize="3rem"
            />
          </div>
        ) : null}
      </DragOverlay>
    </DndContext>
  )
}

function Bin({
  bin,
  placedLabels,
}: {
  bin: SortPayload['bins'][number]
  placedLabels: string[]
}) {
  const { isOver, setNodeRef } = useDroppable({ id: bin.id })
  return (
    <div
      ref={setNodeRef}
      className={
        'min-h-[10rem] rounded-kid border-4 p-3 transition ' +
        (isOver
          ? 'border-brand-500 bg-brand-50'
          : 'border-dashed border-slate-300 bg-white/70')
      }
    >
      <div className="mb-2 text-center text-2xl font-bold text-slate-800">
        {bin.emoji ? `${bin.emoji} ` : ''}
        {bin.label}
      </div>
      <div className="flex flex-wrap justify-center gap-1">
        {placedLabels.map((label) => (
          <motion.span
            key={label}
            initial={{ scale: 0.5, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            className="rounded-full bg-emerald-500 px-2 py-1 text-xs font-bold text-white"
          >
            {label}
          </motion.span>
        ))}
      </div>
    </div>
  )
}

function DraggableItem({
  id,
  image,
  shaking,
  hidden,
  onTap,
}: {
  id: string
  image: string
  shaking: boolean
  hidden: boolean
  onTap: () => void
}) {
  const { attributes, listeners, setNodeRef, isDragging } = useDraggable({ id })
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
      className="flex h-20 w-20 cursor-grab items-center justify-center rounded-kid bg-white shadow-kid hover:scale-105 active:cursor-grabbing"
      aria-label={id}
    >
      <ItemImage image={image} alt={id} className="h-16 w-16" emojiSize="3rem" />
    </motion.button>
  )
}

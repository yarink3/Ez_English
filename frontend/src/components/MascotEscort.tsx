import { AnimatePresence, motion } from 'framer-motion'
import { useTranslation } from 'react-i18next'
import type { CharacterDto } from '../lib/apiTypes'

export type MascotMood = 'idle' | 'happy' | 'sad' | 'celebrating'

type Props = {
  character: CharacterDto | undefined
  mood: MascotMood
  message?: string
}

const moodVariants = {
  idle:        { y: [0, -6, 0], rotate: 0,   transition: { repeat: Infinity, duration: 2.2 } },
  happy:       { y: [0, -22, 0], rotate: [0, -8, 8, 0], transition: { duration: 0.6 } },
  sad:         { x: [0, -8, 8, -6, 6, 0], transition: { duration: 0.4 } },
  celebrating: { scale: [1, 1.25, 1.1, 1.25, 1], rotate: [0, 12, -12, 12, 0], transition: { duration: 0.9 } },
}

export default function MascotEscort({ character, mood, message }: Props) {
  const { i18n } = useTranslation()
  const isHe = i18n.language.startsWith('he')

  if (!character) return null
  const name = isHe ? character.displayNameHe : character.displayNameEn

  return (
    <div className="pointer-events-none fixed bottom-4 z-20 flex items-end gap-3 ltr:left-4 rtl:right-4">
      <motion.img
        src={character.avatarUrl}
        alt={name}
        className="h-24 w-24 drop-shadow-xl"
        animate={moodVariants[mood]}
      />
      <AnimatePresence>
        {message && (
          <motion.div
            key={message}
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            className="mb-6 max-w-xs rounded-kid bg-white px-4 py-2 text-sm font-bold text-slate-800 shadow-kid"
          >
            {message}
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}

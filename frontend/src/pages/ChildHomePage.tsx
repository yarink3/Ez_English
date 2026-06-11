import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useChildren, useCharacters } from '../auth/apiHooks'
import { Category } from '../lib/apiTypes'
import { isCategoryUnlocked } from '../lib/progress'

type CategoryCard = {
  category: Category
  emoji: string
  bg: string
}

const cards: CategoryCard[] = [
  { category: Category.Colors,  emoji: '🎨',     bg: 'bg-pink-200'    },
  { category: Category.Animals, emoji: '🐾',     bg: 'bg-emerald-200' },
  { category: Category.Family,  emoji: '👨‍👩‍👧', bg: 'bg-amber-200'   },
  { category: Category.Letters, emoji: '🔤',     bg: 'bg-sky-200'     },
  { category: Category.Numbers, emoji: '🔢',     bg: 'bg-violet-200'  },
]

const categoryName: Record<Category, string> = {
  [Category.Colors]:  'Colors',
  [Category.Animals]: 'Animals',
  [Category.Family]:  'Family',
  [Category.Letters]: 'Letters',
  [Category.Numbers]: 'Numbers',
}

export default function ChildHomePage() {
  const { childId: childIdParam } = useParams<{ childId: string }>()
  const childId = Number(childIdParam)
  const navigate = useNavigate()
  const { t, i18n } = useTranslation()
  const isHe = i18n.language.startsWith('he')

  const { data: children = [], isLoading } = useChildren()
  const { data: characters = [] } = useCharacters()

  const child = children.find((c) => c.id === childId)
  const character = characters.find((c) => c.id === child?.characterId)

  // Re-render when localStorage progress changes (other tab or after lesson complete).
  const [progressTick, setProgressTick] = useState(0)
  useEffect(() => {
    const bump = () => setProgressTick((n) => n + 1)
    window.addEventListener('ezenglish:progress-updated', bump)
    window.addEventListener('storage', bump)
    return () => {
      window.removeEventListener('ezenglish:progress-updated', bump)
      window.removeEventListener('storage', bump)
    }
  }, [])
  // Reference progressTick so React tracks it in the render dependency.
  void progressTick

  if (isLoading) return <p className="p-6 text-slate-500">{t('common.loading')}…</p>
  if (!child) {
    return (
      <div className="p-6">
        <p className="mb-3 text-red-600">{t('play.childNotFound')}</p>
        <Link to="/" className="text-brand-600 underline">← {t('nav.dashboard')}</Link>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-amber-100 via-rose-100 to-sky-100">
      <header className="flex items-center justify-between bg-white/80 px-6 py-4 shadow">
        <Link to="/" className="text-sm font-semibold text-brand-600 hover:underline">
          ← {t('nav.dashboard')}
        </Link>
        <h1 className="text-lg font-bold text-slate-800" dir="auto">
          {t('play.hi', { name: child.displayName })}
        </h1>
        <div className="w-24" />
      </header>

      <main className="mx-auto max-w-3xl p-6">
        <div className="mb-6 flex flex-col items-center text-center">
          {character && (
            <img src={character.avatarUrl} alt="" className="h-32 w-32 drop-shadow-lg" />
          )}
          <p className="mt-3 text-lg font-bold text-slate-700">
            {character && (isHe ? character.displayNameHe : character.displayNameEn)}
          </p>
          <p className="text-sm text-slate-500">{t('play.pickActivity')}</p>
        </div>

        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
          {cards.map((c) => {
            const unlocked = isCategoryUnlocked(childId, c.category)
            return (
              <button
                key={c.category}
                type="button"
                disabled={!unlocked}
                onClick={() => navigate(`/play/${childId}/${c.category}`)}
                className={
                  'flex flex-col items-center gap-2 rounded-kid p-6 shadow-kid transition ' +
                  c.bg +
                  (unlocked
                    ? ' hover:scale-105'
                    : ' cursor-not-allowed opacity-50')
                }
              >
                <span className="text-5xl">{unlocked ? c.emoji : '🔒'}</span>
                <span className="font-bold text-slate-800">
                  {t(`categories.${categoryName[c.category]}`, {
                    defaultValue: categoryName[c.category],
                  })}
                </span>
                {!unlocked && (
                  <span className="text-xs text-slate-600">{t('play.locked')}</span>
                )}
              </button>
            )
          })}
        </div>
      </main>
    </div>
  )
}

import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams, useSearchParams } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import {
  useChildLessons,
  useLesson,
  useChildren,
  useCharacters,
  useSubmitProgress,
} from '../auth/apiHooks'
import DragMatchGame, { type DragMatchResult } from '../components/DragMatchGame'
import MascotEscort, { type MascotMood } from '../components/MascotEscort'
import { Category, LessonItemKind, type DragMatchPayload } from '../lib/apiTypes'

const CATEGORY_VALUES = new Set<number>(Object.values(Category))

export default function PlayLessonPage() {
  const { childId: childIdParam, category: categoryParam } = useParams<{
    childId: string
    category: string
  }>()
  const [search] = useSearchParams()
  const navigate = useNavigate()
  const { t, i18n } = useTranslation()
  const isHe = i18n.language.startsWith('he')

  const childId = Number(childIdParam)
  const categoryNum = Number(categoryParam)
  const category: Category | null = CATEGORY_VALUES.has(categoryNum)
    ? (categoryNum as Category)
    : null
  const requestedLessonId = search.get('lessonId') ? Number(search.get('lessonId')) : null

  const { data: children = [] } = useChildren()
  const { data: characters = [] } = useCharacters()
  const { data: lessons = [], isLoading: lessonsLoading } = useChildLessons(childId, category)

  const child = children.find((c) => c.id === childId)
  const character = characters.find((c) => c.id === child?.characterId)

  const lessonId = requestedLessonId ?? lessons[0]?.id ?? null
  const { data: lesson, isLoading: lessonLoading, isError } = useLesson(childId, lessonId)

  const submitProgress = useSubmitProgress(childId)

  const [itemIdx, setItemIdx] = useState(0)
  const [mood, setMood] = useState<MascotMood>('idle')
  const [message, setMessage] = useState<string | undefined>()
  const [scores, setScores] = useState<number[]>([])

  useEffect(() => { setItemIdx(0); setScores([]) }, [lessonId])

  const flash = (m: MascotMood, msg?: string, ms = 1100) => {
    setMood(m)
    setMessage(msg)
    window.setTimeout(() => { setMood('idle'); setMessage(undefined) }, ms)
  }

  if (category == null) {
    return (
      <div className="p-6">
        <p className="text-red-600">{t('play.unknownCategory')}</p>
        <Link to={`/play/${childId}`} className="text-brand-600 underline">
          ← {t('play.backToHome')}
        </Link>
      </div>
    )
  }

  if (lessonsLoading || lessonLoading) {
    return <p className="p-6 text-slate-500">{t('common.loading')}…</p>
  }

  if (!lessons.length || !lessonId) {
    return (
      <div className="p-6">
        <p className="mb-3 text-slate-700">{t('play.noLessons')}</p>
        <Link to={`/play/${childId}`} className="text-brand-600 underline">
          ← {t('play.backToHome')}
        </Link>
      </div>
    )
  }

  if (isError || !lesson) {
    return (
      <div className="p-6">
        <p className="mb-3 text-red-600">{t('play.lessonLoadError')}</p>
        <Link to={`/play/${childId}`} className="text-brand-600 underline">
          ← {t('play.backToHome')}
        </Link>
      </div>
    )
  }

  const items = lesson.items
  const currentItem = items[itemIdx]
  const done = itemIdx >= items.length

  if (done) {
    const total = scores.reduce((a, b) => a + b, 0)
    const avg = scores.length ? Math.round(total / scores.length) : 0
    return (
      <div className="min-h-screen bg-gradient-to-br from-emerald-100 via-amber-100 to-rose-100 p-6">
        <div className="mx-auto max-w-md rounded-kid bg-white p-8 text-center shadow-kid">
          <div className="text-6xl">🎉</div>
          <h2 className="mt-4 text-2xl font-bold text-slate-800">
            {t('play.lessonComplete', { title: isHe ? lesson.titleHe : lesson.titleEn })}
          </h2>
          <p className="mt-2 text-lg text-slate-600">
            {t('play.averageScore', { score: avg })}
          </p>
          <div className="mt-6 flex justify-center gap-3">
            <button
              onClick={() => navigate(`/play/${childId}`)}
              className="rounded-kid bg-brand-500 px-5 py-3 font-bold text-white shadow-kid hover:bg-brand-600"
            >
              {t('play.backToHome')}
            </button>
          </div>
        </div>
        <MascotEscort character={character} mood="celebrating" />
      </div>
    )
  }

  const handleAttempt = (correct: boolean) => {
    if (correct) flash('happy', t('play.greatJob'), 800)
    else        flash('sad',   t('play.tryAgain'), 800)
  }

  const handleItemComplete = async (result: DragMatchResult) => {
    setScores((s) => [...s, result.score])
    flash('celebrating', t('play.wellDone'), 1200)
    try {
      await submitProgress.mutateAsync({ lessonItemId: currentItem.id, score: result.score })
    } catch {
      // non-blocking; we still let the kid continue
    }
    window.setTimeout(() => setItemIdx((i) => i + 1), 1300)
  }

  const prompt = isHe && currentItem.promptHe ? currentItem.promptHe : currentItem.promptEn

  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-100 via-pink-100 to-amber-100">
      <header className="flex items-center justify-between bg-white/80 px-6 py-3 shadow">
        <Link to={`/play/${childId}`} className="text-sm font-semibold text-brand-600 hover:underline">
          ← {t('play.backToHome')}
        </Link>
        <span className="text-sm text-slate-500">
          {t('play.itemProgress', { current: itemIdx + 1, total: items.length })}
        </span>
        <div className="w-20" />
      </header>

      <main className="mx-auto max-w-2xl p-6">
        <h2 className="mb-2 text-center text-2xl font-bold text-slate-800">
          {isHe ? lesson.titleHe : lesson.titleEn}
        </h2>
        <p className="mb-6 text-center text-lg text-slate-600" dir="auto">{prompt}</p>

        {currentItem.kind === LessonItemKind.DragMatch ? (
          <DragMatchGame
            key={currentItem.id}
            payload={currentItem.payload as DragMatchPayload}
            onAttempt={handleAttempt}
            onComplete={handleItemComplete}
          />
        ) : (
          <p className="text-center text-slate-500">
            {t('play.unsupportedKind', { kind: currentItem.kind })}
          </p>
        )}
      </main>

      <MascotEscort character={character} mood={mood} message={message} />
    </div>
  )
}

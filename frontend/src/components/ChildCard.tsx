import { useTranslation } from 'react-i18next'
import { Link } from 'react-router-dom'
import type { CharacterDto, ChildDto, Level } from '../lib/apiTypes'
import { levelLabel } from '../lib/apiTypes'

function ageYears(birthDateIso: string): number {
  const b = new Date(birthDateIso)
  const t = new Date()
  let age = t.getFullYear() - b.getFullYear()
  const md = t.getMonth() - b.getMonth()
  if (md < 0 || (md === 0 && t.getDate() < b.getDate())) age--
  return Math.max(0, age)
}

type Props = {
  child: ChildDto
  character?: CharacterDto
  onDelete: (childId: number) => void
  deleting: boolean
}

export default function ChildCard({ child, character, onDelete, deleting }: Props) {
  const { t, i18n } = useTranslation()
  const isHe = i18n.language.startsWith('he')
  const age = ageYears(child.birthDate)
  const characterName = character ? (isHe ? character.displayNameHe : character.displayNameEn) : ''

  const summaryLevel: Level | undefined = child.categoryLevels[0]?.level

  return (
    <article className="flex items-start gap-4 rounded-kid bg-white p-4 shadow-kid">
      {character && (
        <img src={character.avatarUrl} alt="" className="h-20 w-20 shrink-0" />
      )}
      <div className="flex-1">
        <h3 className="text-lg font-bold text-slate-800" dir="auto">
          {child.displayName}
        </h3>
        <p className="text-sm text-slate-500">
          {t('child.ageYears', { count: age })} · {characterName}
        </p>
        {summaryLevel && (
          <p className="mt-1 text-xs font-semibold uppercase tracking-wide text-brand-600">
            {t('child.levelLabel')}: <span dir="ltr">{levelLabel(summaryLevel)}</span>
          </p>
        )}
      </div>
      <div className="flex shrink-0 items-center gap-2">
        <Link
          to={`/play/${child.id}`}
          className="rounded-kid bg-brand-500 px-4 py-2 text-sm font-bold text-white shadow-kid hover:bg-brand-600"
        >
          ▶ {t('child.play')}
        </Link>
        <button
          type="button"
          onClick={() => onDelete(child.id)}
          disabled={deleting}
          aria-label={t('child.delete')}
          className="rounded-full p-2 text-slate-400 hover:bg-red-50 hover:text-red-600 disabled:opacity-50"
          title={t('child.delete')}
        >🗑</button>
      </div>
    </article>
  )
}

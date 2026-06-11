import type { CharacterDto } from '../lib/apiTypes'
import { useTranslation } from 'react-i18next'

type Props = {
  characters: CharacterDto[]
  selectedId: number | null
  onSelect: (id: number) => void
}

export default function CharacterPicker({ characters, selectedId, onSelect }: Props) {
  const { i18n } = useTranslation()
  const isHe = i18n.language.startsWith('he')

  return (
    <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
      {characters.map((c) => {
        const selected = c.id === selectedId
        const name = isHe ? c.displayNameHe : c.displayNameEn
        return (
          <button
            key={c.id}
            type="button"
            onClick={() => onSelect(c.id)}
            aria-pressed={selected}
            className={
              'flex flex-col items-center gap-2 rounded-kid border-2 p-3 transition ' +
              (selected
                ? 'border-brand-500 bg-brand-50 shadow-kid'
                : 'border-slate-200 bg-white hover:border-brand-300')
            }
          >
            <img src={c.avatarUrl} alt="" className="h-20 w-20" />
            <span className="text-sm font-semibold">{name}</span>
          </button>
        )
      })}
    </div>
  )
}

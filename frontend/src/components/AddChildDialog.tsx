import { useEffect, useState, type FormEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { useCharacters, useCreateChild } from '../auth/apiHooks'
import { ApiError } from '../lib/api'
import CharacterPicker from './CharacterPicker'

type Props = {
  open: boolean
  onClose: () => void
}

const todayIso = () => new Date().toISOString().slice(0, 10)
const minIso   = () => { const d = new Date(); d.setFullYear(d.getFullYear() - 12); return d.toISOString().slice(0, 10) }
const maxIso   = () => { const d = new Date(); d.setFullYear(d.getFullYear() - 3);  return d.toISOString().slice(0, 10) }

export default function AddChildDialog({ open, onClose }: Props) {
  const { t } = useTranslation()
  const { data: characters = [] } = useCharacters()
  const createChild = useCreateChild()

  const [displayName, setDisplayName] = useState('')
  const [birthDate, setBirthDate]     = useState('')
  const [characterId, setCharacterId] = useState<number | null>(null)
  const [pin, setPin]                 = useState('')
  const [errorMsg, setErrorMsg]       = useState<string | null>(null)

  useEffect(() => {
    if (open) {
      setDisplayName('')
      setBirthDate('')
      setCharacterId(characters[0]?.id ?? null)
      setPin('')
      setErrorMsg(null)
    }
  }, [open, characters])

  if (!open) return null

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setErrorMsg(null)

    if (!displayName.trim())            { setErrorMsg(t('child.errors.nameRequired'));      return }
    if (!birthDate)                     { setErrorMsg(t('child.errors.birthDateRequired')); return }
    if (birthDate < minIso() || birthDate > todayIso()) {
      setErrorMsg(t('child.errors.birthDateOutOfRange'));                                    return
    }
    if (characterId == null)            { setErrorMsg(t('child.errors.characterRequired')); return }
    if (pin && !/^\d{4}$/.test(pin))    { setErrorMsg(t('child.errors.pinFormat'));         return }

    try {
      await createChild.mutateAsync({
        displayName: displayName.trim(),
        birthDate,
        characterId,
        pin: pin || undefined,
      })
      onClose()
    } catch (err) {
      const apiMsg = err instanceof ApiError
        ? (err.body as { error?: string } | undefined)?.error
        : undefined
      setErrorMsg(apiMsg || t('child.errors.generic'))
    }
  }

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
      role="dialog"
      aria-modal="true"
    >
      <div className="w-full max-w-lg rounded-kid bg-white p-6 shadow-2xl">
        <div className="mb-4 flex items-center justify-between">
          <h3 className="text-xl font-bold text-slate-800">{t('child.addTitle')}</h3>
          <button
            onClick={onClose}
            aria-label={t('common.close')}
            className="rounded-full p-2 text-slate-500 hover:bg-slate-100"
          >✕</button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <label className="block">
            <span className="mb-1 block text-sm font-semibold">{t('child.nameLabel')}</span>
            <input
              value={displayName}
              onChange={(e) => setDisplayName(e.target.value)}
              maxLength={64}
              className="w-full rounded-xl border-2 border-slate-200 px-4 py-2.5 outline-none focus:border-brand-500"
              autoFocus
            />
          </label>

          <label className="block">
            <span className="mb-1 block text-sm font-semibold">
              {t('child.birthDateLabel')}{' '}
              <span className="text-xs text-slate-400">{t('child.birthDateHint')}</span>
            </span>
            <input
              type="date"
              value={birthDate}
              onChange={(e) => setBirthDate(e.target.value)}
              min={minIso()}
              max={maxIso()}
              className="w-full rounded-xl border-2 border-slate-200 px-4 py-2.5 outline-none focus:border-brand-500"
            />
          </label>

          <div>
            <span className="mb-2 block text-sm font-semibold">{t('child.characterLabel')}</span>
            <CharacterPicker
              characters={characters}
              selectedId={characterId}
              onSelect={setCharacterId}
            />
          </div>

          <label className="block">
            <span className="mb-1 block text-sm font-semibold">
              {t('child.pinLabel')}{' '}
              <span className="text-xs text-slate-400">{t('child.pinHint')}</span>
            </span>
            <input
              inputMode="numeric"
              pattern="\d{4}"
              maxLength={4}
              value={pin}
              onChange={(e) => setPin(e.target.value.replace(/\D/g, ''))}
              placeholder="••••"
              className="w-32 rounded-xl border-2 border-slate-200 px-4 py-2.5 text-center text-xl tracking-widest outline-none focus:border-brand-500"
            />
          </label>

          {errorMsg && (
            <p className="text-sm font-medium text-red-600" role="alert">{errorMsg}</p>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="rounded-kid border-2 border-slate-200 px-4 py-2.5 font-semibold text-slate-700 hover:bg-slate-50"
            >
              {t('common.cancel')}
            </button>
            <button
              type="submit"
              disabled={createChild.isPending}
              className="rounded-kid bg-brand-500 px-5 py-2.5 font-bold text-white shadow-kid hover:bg-brand-600 disabled:opacity-50"
            >
              {createChild.isPending ? t('common.saving') : t('child.submit')}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

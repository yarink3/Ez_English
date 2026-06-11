import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useAuth } from '../auth/AuthProvider'
import { useChildren, useCharacters, useDeleteChild } from '../auth/apiHooks'
import AddChildDialog from '../components/AddChildDialog'
import ChildCard from '../components/ChildCard'

export default function DashboardPage() {
  const { t, i18n } = useTranslation()
  const { user, signOutNow } = useAuth()
  const { data: children = [], isLoading, isError, error } = useChildren()
  const { data: characters = [] } = useCharacters()
  const deleteChild = useDeleteChild()
  const [dialogOpen, setDialogOpen] = useState(false)

  const charsById = new Map(characters.map((c) => [c.id, c]))

  const toggleLang = () => {
    void i18n.changeLanguage(i18n.language.startsWith('he') ? 'en' : 'he')
  }

  const handleDelete = (childId: number) => {
    if (window.confirm(t('child.confirmDelete'))) {
      deleteChild.mutate(childId)
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-100 via-brand-50 to-amber-100">
      <header className="flex items-center justify-between bg-white/80 px-6 py-4 shadow">
        <h1 className="font-display text-2xl font-bold text-brand-600">{t('appName')}</h1>
        <div className="flex items-center gap-3">
          <button
            onClick={toggleLang}
            className="rounded-xl border border-slate-300 px-3 py-1.5 text-sm font-semibold hover:bg-slate-50"
          >
            {i18n.language.startsWith('he') ? 'EN' : 'עב'}
          </button>
          <button
            onClick={() => void signOutNow()}
            className="rounded-xl bg-brand-500 px-3 py-1.5 text-sm font-semibold text-white hover:bg-brand-600"
          >
            {t('nav.logout')}
          </button>
        </div>
      </header>

      <main className="mx-auto max-w-3xl p-6">
        <div className="mb-6 flex items-end justify-between gap-4">
          <div>
            <h2 className="text-3xl font-bold text-slate-800">
              {t('dashboard.welcome', { name: user?.displayName || user?.email || '👋' })}
            </h2>
            <p className="mt-1 text-slate-600">{t('dashboard.subtitle')}</p>
          </div>
          <button
            onClick={() => setDialogOpen(true)}
            className="shrink-0 rounded-kid bg-brand-500 px-5 py-3 font-bold text-white shadow-kid hover:bg-brand-600"
          >
            + {t('dashboard.addChild')}
          </button>
        </div>

        {isLoading && <p className="text-slate-500">{t('common.loading')}…</p>}

        {isError && (
          <div className="rounded-kid border-2 border-red-200 bg-red-50 p-4 text-red-700">
            {t('dashboard.loadError')}
            <p className="mt-1 text-xs opacity-75">{String((error as Error)?.message ?? '')}</p>
          </div>
        )}

        {!isLoading && !isError && children.length === 0 && (
          <div className="rounded-kid border-2 border-dashed border-brand-300 bg-white/70 p-8 text-center">
            <div className="text-5xl">🦄</div>
            <p className="mt-3 text-slate-700">{t('dashboard.noChildrenYet')}</p>
            <button
              onClick={() => setDialogOpen(true)}
              className="mt-4 rounded-kid bg-brand-500 px-5 py-3 font-bold text-white shadow-kid hover:bg-brand-600"
            >
              {t('dashboard.addChild')}
            </button>
          </div>
        )}

        {children.length > 0 && (
          <div className="space-y-3">
            {children.map((c) => (
              <ChildCard
                key={c.id}
                child={c}
                character={charsById.get(c.characterId)}
                onDelete={handleDelete}
                deleting={deleteChild.isPending && deleteChild.variables === c.id}
              />
            ))}
          </div>
        )}
      </main>

      <AddChildDialog open={dialogOpen} onClose={() => setDialogOpen(false)} />
    </div>
  )
}

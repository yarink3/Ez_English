import { useState, type FormEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthProvider'
import { mapAuthError } from '../auth/errors'
import { GoogleIcon } from '../components/GoogleIcon'

export default function LoginPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { signInWithEmail, signInWithGoogle } = useAuth()

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [errorKey, setErrorKey] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  const handleEmailSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setErrorKey(null)
    if (!email) return setErrorKey('auth.errors.emailRequired')
    if (!password) return setErrorKey('auth.errors.passwordRequired')
    setBusy(true)
    try {
      await signInWithEmail(email, password)
      navigate('/')
    } catch (err) {
      setErrorKey(mapAuthError(err))
    } finally {
      setBusy(false)
    }
  }

  const handleGoogle = async () => {
    setErrorKey(null)
    setBusy(true)
    try {
      await signInWithGoogle()
      navigate('/')
    } catch (err) {
      setErrorKey(mapAuthError(err))
    } finally {
      setBusy(false)
    }
  }

  return (
    <AuthShell title={t('auth.loginTitle')}>
      <form onSubmit={handleEmailSubmit} className="space-y-4">
        <Field
          id="email"
          type="email"
          label={t('auth.email')}
          value={email}
          onChange={setEmail}
          autoComplete="email"
        />
        <Field
          id="password"
          type="password"
          label={t('auth.password')}
          value={password}
          onChange={setPassword}
          autoComplete="current-password"
        />

        {errorKey && (
          <p className="text-sm font-medium text-red-600" role="alert">
            {t(errorKey)}
          </p>
        )}

        <button
          type="submit"
          disabled={busy}
          className="w-full rounded-kid bg-brand-500 px-4 py-3 text-lg font-bold text-white shadow-kid transition hover:bg-brand-600 disabled:opacity-50"
        >
          {t('auth.submitLogin')}
        </button>
      </form>

      <Divider label={t('auth.or')} />

      <GoogleButton onClick={handleGoogle} disabled={busy} label={t('auth.googleButton')} />

      <p className="mt-6 text-center text-sm text-slate-600">
        <Link to="/register" className="font-semibold text-brand-600 hover:underline">
          {t('auth.switchToRegister')}
        </Link>
      </p>
    </AuthShell>
  )
}

/* ---------- shared bits used by Login + Register ---------- */

export function AuthShell({ title, children }: { title: string; children: React.ReactNode }) {
  const { t } = useTranslation()
  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-100 via-brand-50 to-amber-100 flex items-center justify-center p-4">
      <div className="w-full max-w-md rounded-kid bg-white p-8 shadow-kid">
        <div className="mb-6 text-center">
          <div className="text-5xl">📚🦄</div>
          <h1 className="mt-2 font-display text-3xl font-bold text-brand-600">
            {t('appName')}
          </h1>
          <p className="text-sm text-slate-500">{t('tagline')}</p>
        </div>
        <h2 className="mb-4 text-center text-xl font-bold text-slate-800">{title}</h2>
        {children}
      </div>
    </div>
  )
}

export function Field({
  id,
  label,
  type,
  value,
  onChange,
  autoComplete,
}: {
  id: string
  label: string
  type: string
  value: string
  onChange: (v: string) => void
  autoComplete?: string
}) {
  return (
    <label className="block">
      <span className="mb-1 block text-sm font-semibold text-slate-700">{label}</span>
      <input
        id={id}
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        autoComplete={autoComplete}
        className="w-full rounded-xl border-2 border-slate-200 px-4 py-2.5 text-base outline-none transition focus:border-brand-500 focus:ring-2 focus:ring-brand-200"
      />
    </label>
  )
}

export function Divider({ label }: { label: string }) {
  return (
    <div className="my-5 flex items-center gap-3">
      <span className="h-px flex-1 bg-slate-200" />
      <span className="text-xs uppercase tracking-wider text-slate-400">{label}</span>
      <span className="h-px flex-1 bg-slate-200" />
    </div>
  )
}

export function GoogleButton({
  onClick,
  disabled,
  label,
}: {
  onClick: () => void
  disabled?: boolean
  label: string
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      className="flex w-full items-center justify-center gap-3 rounded-kid border-2 border-slate-200 bg-white px-4 py-3 text-base font-semibold text-slate-700 transition hover:bg-slate-50 disabled:opacity-50"
    >
      <GoogleIcon />
      <span>{label}</span>
    </button>
  )
}

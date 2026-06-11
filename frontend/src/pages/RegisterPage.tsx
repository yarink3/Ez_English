import { useState, type FormEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthProvider'
import { mapAuthError } from '../auth/errors'
import { AuthShell, Divider, Field, GoogleButton } from './LoginPage'

export default function RegisterPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { signUpWithEmail, signInWithGoogle } = useAuth()

  const [displayName, setDisplayName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [errorKey, setErrorKey] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setErrorKey(null)
    if (!email) return setErrorKey('auth.errors.emailRequired')
    if (!password) return setErrorKey('auth.errors.passwordRequired')
    if (password.length < 6) return setErrorKey('auth.errors.passwordTooShort')
    setBusy(true)
    try {
      await signUpWithEmail(email, password, displayName || undefined)
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
    <AuthShell title={t('auth.registerTitle')}>
      <form onSubmit={handleSubmit} className="space-y-4">
        <Field
          id="displayName"
          type="text"
          label={t('auth.displayName')}
          value={displayName}
          onChange={setDisplayName}
          autoComplete="name"
        />
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
          autoComplete="new-password"
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
          {t('auth.submitRegister')}
        </button>
      </form>

      <Divider label={t('auth.or')} />

      <GoogleButton onClick={handleGoogle} disabled={busy} label={t('auth.googleButton')} />

      <p className="mt-6 text-center text-sm text-slate-600">
        <Link to="/login" className="font-semibold text-brand-600 hover:underline">
          {t('auth.switchToLogin')}
        </Link>
      </p>
    </AuthShell>
  )
}

import type { FirebaseError } from 'firebase/app'

export function mapAuthError(err: unknown): string {
  const code = (err as FirebaseError)?.code
  switch (code) {
    case 'auth/invalid-credential':
    case 'auth/wrong-password':
    case 'auth/user-not-found':
      return 'auth.errors.invalidCredentials'
    case 'auth/email-already-in-use':
      return 'auth.errors.emailInUse'
    case 'auth/popup-closed-by-user':
    case 'auth/cancelled-popup-request':
      return 'auth.errors.popupClosed'
    default:
      return 'auth.errors.generic'
  }
}


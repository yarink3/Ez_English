import { Component, type ErrorInfo, type ReactNode } from 'react'

type Props = { children: ReactNode }
type State = { error: Error | null }

/**
 * Catches render-time errors anywhere below it and shows a friendly
 * fallback instead of a blank white page. Kids/parents at least see
 * something they can act on (and we get the stack in the console).
 */
export default class ErrorBoundary extends Component<Props, State> {
  state: State = { error: null }

  static getDerivedStateFromError(error: Error): State {
    return { error }
  }

  componentDidCatch(error: Error, info: ErrorInfo): void {
    console.error('[ErrorBoundary] uncaught render error:', error, info)
  }

  private handleReset = () => {
    this.setState({ error: null })
  }

  render() {
    if (!this.state.error) return this.props.children
    return (
      <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-rose-100 via-amber-100 to-sky-100 p-6">
        <div className="mx-auto max-w-md rounded-kid bg-white p-8 text-center shadow-kid">
          <div className="text-5xl">😅</div>
          <h2 className="mt-4 text-xl font-bold text-slate-800">
            Oops — something went wrong
          </h2>
          <p className="mt-2 text-sm text-slate-600">
            {this.state.error.message || 'Unknown error'}
          </p>
          <div className="mt-6 flex flex-wrap justify-center gap-3">
            <button
              type="button"
              onClick={this.handleReset}
              className="rounded-kid bg-brand-500 px-5 py-2 font-bold text-white shadow-kid hover:bg-brand-600"
            >
              Try again
            </button>
            <button
              type="button"
              onClick={() => window.location.assign('/')}
              className="rounded-kid border-2 border-slate-200 bg-white px-5 py-2 font-bold text-slate-700 hover:bg-slate-50"
            >
              Go home
            </button>
          </div>
        </div>
      </div>
    )
  }
}

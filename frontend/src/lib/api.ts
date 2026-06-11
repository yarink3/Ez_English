import { auth } from './firebase'

const BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5181').replace(/\/+$/, '')

export class ApiError extends Error {
  readonly status: number
  readonly body?: unknown
  constructor(status: number, message: string, body?: unknown) {
    super(message)
    this.status = status
    this.body = body
  }
}

async function authHeader(): Promise<Record<string, string>> {
  const user = auth.currentUser
  if (!user) return {}
  const token = await user.getIdToken()
  return { Authorization: `Bearer ${token}` }
}

type Method = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH'

async function request<T>(method: Method, path: string, body?: unknown): Promise<T> {
  const headers: Record<string, string> = {
    Accept: 'application/json',
    ...(await authHeader()),
  }
  if (body !== undefined) headers['Content-Type'] = 'application/json'

  const res = await fetch(`${BASE_URL}${path}`, {
    method,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body),
  })

  if (!res.ok) {
    let parsed: unknown
    try { parsed = await res.json() } catch { /* ignore */ }
    throw new ApiError(res.status, `HTTP ${res.status} ${res.statusText}`, parsed)
  }

  if (res.status === 204) return undefined as T
  return (await res.json()) as T
}

export const api = {
  get:    <T>(path: string)                 => request<T>('GET', path),
  post:   <T>(path: string, body: unknown)  => request<T>('POST', path, body),
  put:    <T>(path: string, body: unknown)  => request<T>('PUT', path, body),
  del:    <T>(path: string)                 => request<T>('DELETE', path),
}

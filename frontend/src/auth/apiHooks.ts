import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../lib/api'
import type {
  CharacterDto,
  ChildDto,
  CreateChildRequest,
  LessonDetailDto,
  LessonSummaryDto,
  MeResponse,
  ProgressDto,
  SubmitProgressRequest,
  Category,
} from '../lib/apiTypes'
import { useAuth } from './AuthProvider'

const keys = {
  me:         ['me'] as const,
  children:   ['children'] as const,
  characters: ['characters'] as const,
  childLessons: (childId: number, category: Category) =>
    ['children', childId, 'lessons', category] as const,
  lesson: (childId: number, lessonId: number) =>
    ['children', childId, 'lesson', lessonId] as const,
  childProgress: (childId: number) =>
    ['children', childId, 'progress'] as const,
}

/** GET /api/me — current parent + their children. Disabled when logged out. */
export function useMe() {
  const { user } = useAuth()
  return useQuery({
    queryKey: keys.me,
    queryFn: () => api.get<MeResponse>('/api/me'),
    enabled: !!user,
  })
}

export function useChildren() {
  const { user } = useAuth()
  return useQuery({
    queryKey: keys.children,
    queryFn: () => api.get<ChildDto[]>('/api/children'),
    enabled: !!user,
  })
}

export function useCharacters() {
  const { user } = useAuth()
  return useQuery({
    queryKey: keys.characters,
    queryFn: () => api.get<CharacterDto[]>('/api/characters'),
    enabled: !!user,
    staleTime: 5 * 60 * 1000, // characters are basically static
  })
}

export function useCreateChild() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: CreateChildRequest) => api.post<ChildDto>('/api/children', body),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: keys.children })
      void qc.invalidateQueries({ queryKey: keys.me })
    },
  })
}

export function useDeleteChild() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (childId: number) => api.del<void>(`/api/children/${childId}`),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: keys.children })
      void qc.invalidateQueries({ queryKey: keys.me })
    },
  })
}

export function useChildLessons(childId: number | null, category: Category | null) {
  const { user } = useAuth()
  return useQuery({
    queryKey: childId != null && category != null
      ? keys.childLessons(childId, category)
      : ['children', 'lessons', 'disabled'],
    queryFn: () => api.get<LessonSummaryDto[]>(
      `/api/children/${childId}/lessons?category=${category}`),
    enabled: !!user && childId != null && category != null,
  })
}

export function useLesson(childId: number | null, lessonId: number | null) {
  const { user } = useAuth()
  return useQuery({
    queryKey: childId != null && lessonId != null
      ? keys.lesson(childId, lessonId)
      : ['children', 'lesson', 'disabled'],
    queryFn: () => api.get<LessonDetailDto>(
      `/api/children/${childId}/lessons/${lessonId}`),
    enabled: !!user && childId != null && lessonId != null,
  })
}

export function useChildProgress(childId: number | null) {
  const { user } = useAuth()
  return useQuery({
    queryKey: childId != null ? keys.childProgress(childId) : ['children', 'progress', 'disabled'],
    queryFn: () => api.get<ProgressDto[]>(`/api/children/${childId}/progress`),
    enabled: !!user && childId != null,
  })
}

export function useSubmitProgress(childId: number) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: SubmitProgressRequest) =>
      api.post<ProgressDto>(`/api/children/${childId}/progress`, body),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: keys.childProgress(childId) })
    },
  })
}

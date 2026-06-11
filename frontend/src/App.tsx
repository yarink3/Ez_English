import { Navigate, Route, Routes } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import DashboardPage from './pages/DashboardPage'
import ChildHomePage from './pages/ChildHomePage'
import PlayLessonPage from './pages/PlayLessonPage'
import { RequireAuth } from './auth/RequireAuth'
import ErrorBoundary from './components/ErrorBoundary'

export default function App() {
  return (
    <ErrorBoundary>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route
          path="/"
          element={
            <RequireAuth>
              <DashboardPage />
            </RequireAuth>
          }
        />
        <Route
          path="/play/:childId"
          element={
            <RequireAuth>
              <ChildHomePage />
            </RequireAuth>
          }
        />
        <Route
          path="/play/:childId/:category"
          element={
            <RequireAuth>
              <PlayLessonPage />
            </RequireAuth>
          }
        />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </ErrorBoundary>
  )
}

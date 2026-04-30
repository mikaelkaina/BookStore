import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import Spinner from '../ui/Spinner'

interface ProtectedRouteProps {
  children: React.ReactNode
  requireAdmin?: boolean
}

export default function ProtectedRoute({
  children,
  requireAdmin = false,
}: ProtectedRouteProps) {
  const { isAuthenticated, isAdmin, isLoading } = useAuth()
  const location = useLocation()

  if (isLoading) return <Spinner />

  if (!isAuthenticated)
    return <Navigate to="/login" state={{ from: location }} replace />

  if (requireAdmin && !isAdmin)
    return <Navigate to="/" replace />

  return <>{children}</>
}
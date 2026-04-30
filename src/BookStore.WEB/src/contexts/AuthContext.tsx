import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react'
import type { AuthUser, AuthResponse } from '../types'
import { authApi } from '../api/endpoints/auth'

interface AuthContextData {
  user: AuthUser | null
  isAuthenticated: boolean
  isAdmin: boolean
  isLoading: boolean
  login: (response: AuthResponse) => void
  logout: () => void
}

const AuthContext = createContext<AuthContextData>({} as AuthContextData)

const STORAGE_KEYS = {
  accessToken: 'bookstore_access_token',
  refreshToken: 'bookstore_refresh_token',
  user: 'bookstore_user',
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  // Recupera sessão salva ao iniciar
  useEffect(() => {
    const savedUser = localStorage.getItem(STORAGE_KEYS.user)
    const accessToken = localStorage.getItem(STORAGE_KEYS.accessToken)

    if (savedUser && accessToken) {
      setUser(JSON.parse(savedUser))
    }
    setIsLoading(false)
  }, [])

  const login = useCallback((response: AuthResponse) => {
    const authUser: AuthUser = {
      userId: response.userId,
      email: response.email,
      firstName: response.firstName,
      lastName: response.lastName,
      customerId: response.customerId,
      roles: response.roles,
    }

    localStorage.setItem(STORAGE_KEYS.accessToken, response.accessToken)
    localStorage.setItem(STORAGE_KEYS.refreshToken, response.refreshToken)
    localStorage.setItem(STORAGE_KEYS.user, JSON.stringify(authUser))

    setUser(authUser)
  }, [])

  const logout = useCallback(() => {
    localStorage.removeItem(STORAGE_KEYS.accessToken)
    localStorage.removeItem(STORAGE_KEYS.refreshToken)
    localStorage.removeItem(STORAGE_KEYS.user)
    localStorage.removeItem('bookstore_session')
    setUser(null)
  }, [])

  return (
    <AuthContext.Provider value={{
      user,
      isAuthenticated: !!user,
      isAdmin: user?.roles.includes('Admin') ?? false,
      isLoading,
      login,
      logout,
    }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  return useContext(AuthContext)
}
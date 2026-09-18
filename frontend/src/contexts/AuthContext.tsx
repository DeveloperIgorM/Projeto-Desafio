import {
  createContext,
  useContext,
  useEffect,
  useRef,
  useState,
  type ReactNode,
} from 'react'
import { keycloak } from '@/lib/keycloak'

interface AuthContextValue {
  initialized: boolean
  authenticated: boolean
  username: string | null
  hasRole: (role: string) => boolean
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [initialized, setInitialized] = useState(false)
  const [authenticated, setAuthenticated] = useState(false)
  // O React StrictMode roda os effects duas vezes em dev, e o keycloak-js
  // nao aceita ser inicializado duas vezes na mesma instancia - esse ref
  // evita o segundo init.
  const initCalled = useRef(false)

  useEffect(() => {
    if (initCalled.current) return
    initCalled.current = true

    keycloak
      .init({
        onLoad: 'login-required',
        pkceMethod: 'S256',
        checkLoginIframe: false,
      })
      .then((auth) => {
        setAuthenticated(auth)
        setInitialized(true)
      })
      .catch((error) => {
        console.error('Falha ao inicializar o Keycloak', error)
        setInitialized(true)
      })

    keycloak.onTokenExpired = () => {
      keycloak.updateToken(30).catch(() => keycloak.login())
    }
  }, [])

  const value: AuthContextValue = {
    initialized,
    authenticated,
    username: (keycloak.tokenParsed?.preferred_username as string | undefined) ?? null,
    hasRole: (role: string) => keycloak.hasRealmRole(role),
    logout: () => keycloak.logout({ redirectUri: window.location.origin }),
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth precisa ser usado dentro de um AuthProvider')
  return ctx
}

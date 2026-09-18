import type { ReactNode } from 'react'
import { useAuth } from '@/contexts/AuthContext'

// A app inteira ja exige login (onLoad: 'login-required'), entao esse
// componente cobre dois casos: a janela entre o app montar e o Keycloak
// terminar de inicializar, e paginas que exigem uma role especifica.
export function ProtectedRoute({
  children,
  role,
}: {
  children: ReactNode
  role?: string
}) {
  const { initialized, authenticated, hasRole } = useAuth()

  if (!initialized) {
    return (
      <div className="flex h-screen items-center justify-center text-sm text-muted-foreground">
        Carregando...
      </div>
    )
  }

  if (!authenticated) {
    return (
      <div className="flex h-screen items-center justify-center text-sm text-muted-foreground">
        Redirecionando para o login...
      </div>
    )
  }

  if (role && !hasRole(role)) {
    return (
      <div className="flex h-screen items-center justify-center text-sm text-muted-foreground">
        Voce nao tem permissao para acessar esta pagina.
      </div>
    )
  }

  return <>{children}</>
}

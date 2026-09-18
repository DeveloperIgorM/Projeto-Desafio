import Keycloak from 'keycloak-js'

// Instancia unica do client - o keycloak-js reclama (e com razao) se voce
// tentar inicializar mais de uma vez, entao esse modulo so existe pra
// garantir que todo mundo importa a mesma referencia.
export const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL as string,
  realm: import.meta.env.VITE_KEYCLOAK_REALM as string,
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID as string,
})

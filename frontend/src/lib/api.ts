import axios from 'axios'
import { keycloak } from './keycloak'

export const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_URL}/api`,
})

// Antes de cada request, garante que o token ainda vale por pelo menos mais
// 30s (o Keycloak renova sozinho se precisar) e anexa o Bearer.
api.interceptors.request.use(async (config) => {
  if (keycloak.authenticated) {
    try {
      await keycloak.updateToken(30)
    } catch {
      keycloak.login()
    }
    config.headers.Authorization = `Bearer ${keycloak.token}`
  }
  return config
})

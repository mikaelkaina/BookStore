import api from '../axios'
import type { AuthResponse } from '../../types'

export interface RegisterPayload {
  firstName: string
  lastName: string
  email: string
  password: string
  document: string
  phone?: string
  birthDate?: string
}

export interface LoginPayload {
  email: string
  password: string
}

export const authApi = {
  register: (payload: RegisterPayload) =>
    api.post<AuthResponse>('/auth/register', payload),

  login: (payload: LoginPayload) =>
    api.post<AuthResponse>('/auth/login', payload),

  refresh: (accessToken: string, refreshToken: string) =>
    api.post<AuthResponse>('/auth/refresh', { accessToken, refreshToken }),
}
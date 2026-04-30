import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(config => {
  const token = localStorage.getItem('bookstore_access_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true

      const accessToken = localStorage.getItem('bookstore_access_token')
      const refreshToken = localStorage.getItem('bookstore_refresh_token')

      if (accessToken && refreshToken) {
        try {
          const { data } = await axios.post('/api/auth/refresh', {
            accessToken,
            refreshToken,
          })

          localStorage.setItem('bookstore_access_token', data.accessToken)
          localStorage.setItem('bookstore_refresh_token', data.refreshToken)

          originalRequest.headers.Authorization = `Bearer ${data.accessToken}`
          return api(originalRequest)
        } catch {
          localStorage.removeItem('bookstore_access_token')
          localStorage.removeItem('bookstore_refresh_token')
          localStorage.removeItem('bookstore_user')
          window.location.href = '/login'
        }
      }
    }

    const message =
      error.response?.data?.detail ||
      error.response?.data?.title ||
      'An unexpected error occurred.'

    console.error(`[API Error] ${error.response?.status}: ${message}`)
    return Promise.reject(error)
  }
)

export default api
import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor de response — trata erros globalmente
api.interceptors.response.use(
  (response) => response,
  (error) => {
    const message =
      error.response?.data?.detail ||
      error.response?.data?.title ||
      'An unexpected error occurred.';

    console.error(`[API Error] ${error.response?.status}: ${message}`);
    return Promise.reject(error);
  }
);

export default api;
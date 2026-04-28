import api from '../axios';
import type { Category } from '../../types';

export const categoriesApi = {
  getAll: () =>
    api.get<Category[]>('/categories'),

  getById: (id: string) =>
    api.get<Category>(`/categories/${id}`),

  create: (payload: { name: string; description?: string }) =>
    api.post<Category>('/categories', payload),

  update: (id: string, payload: { name: string; description?: string }) =>
    api.put<Category>(`/categories/${id}`, { categoryId: id, ...payload }),

  delete: (id: string) =>
    api.delete(`/categories/${id}`),
};
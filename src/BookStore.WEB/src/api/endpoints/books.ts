import api from '../axios';
import type { BookDetail, BookSummary, PagedResponse } from '../../types';

export interface GetBooksParams {
  searchTerm?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  sortByPrice?: boolean;
  ascending?: boolean;
  page?: number;
  pageSize?: number;
}

export interface CreateBookPayload {
  title: string;
  author: string;
  description?: string;
  isbn: string;
  price: number;
  stockQuantity: number;
  pageCount: number;
  coverImageUrl?: string;
  publisher: string;
  publishedDate: string;
  format: number;
  language: string;
  categoryId: string;
}

export const booksApi = {
  getPaged: (params: GetBooksParams) =>
    api.get<PagedResponse<BookSummary>>('/books', { params }),

  getById: (id: string) =>
    api.get<BookDetail>(`/books/${id}`),

  getByCategory: (categoryId: string) =>
    api.get<BookSummary[]>(`/books/category/${categoryId}`),

  create: (payload: CreateBookPayload) =>
    api.post<BookDetail>('/books', payload),

  update: (id: string, payload: Partial<CreateBookPayload>) =>
    api.put<BookDetail>(`/books/${id}`, { bookId: id, ...payload }),

  updatePrice: (id: string, newPrice: number) =>
    api.patch(`/books/${id}/price`, { bookId: id, newPrice }),

  addStock: (id: string, quantity: number) =>
    api.patch(`/books/${id}/stock/add`, { bookId: id, quantity }),

  decrementStock: (id: string, quantity: number) =>
    api.patch(`/books/${id}/stock/decrement`, { bookId: id, quantity }),

  delete: (id: string) =>
    api.delete(`/books/${id}`),
};
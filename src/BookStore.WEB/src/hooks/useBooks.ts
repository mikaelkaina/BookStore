import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { booksApi, type GetBooksParams, type CreateBookPayload } from '../api/endpoints/books'

export const bookKeys = {
  all: ['books'] as const,
  paged: (params: GetBooksParams) => ['books', 'paged', params] as const,
  detail: (id: string) => ['books', 'detail', id] as const,
  byCategory: (categoryId: string) => ['books', 'category', categoryId] as const,
}

export function useBooksPaged(params: GetBooksParams) {
  return useQuery({
    queryKey: bookKeys.paged(params),
    queryFn: () => booksApi.getPaged(params).then(r => r.data),
  })
}

export function useBookById(id: string) {
  return useQuery({
    queryKey: bookKeys.detail(id),
    queryFn: () => booksApi.getById(id).then(r => r.data),
    enabled: !!id,
  })
}

export function useBooksByCategory(categoryId: string) {
  return useQuery({
    queryKey: bookKeys.byCategory(categoryId),
    queryFn: () => booksApi.getByCategory(categoryId).then(r => r.data),
    enabled: !!categoryId,
  })
}

export function useCreateBook() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateBookPayload) => booksApi.create(payload).then(r => r.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: bookKeys.all })
    },
  })
}

export function useDeleteBook() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => booksApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: bookKeys.all })
    },
  })
}

export function useUpdateBookPrice() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, newPrice }: { id: string; newPrice: number }) =>
      booksApi.updatePrice(id, newPrice),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: bookKeys.all })
    },
  })
}

export function useAddStock() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, quantity }: { id: string; quantity: number }) =>
      booksApi.addStock(id, quantity),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: bookKeys.all })
    },
  })
}
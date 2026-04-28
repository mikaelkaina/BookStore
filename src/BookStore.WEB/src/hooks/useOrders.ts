import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ordersApi, type CreateOrderPayload } from '../api/endpoints/orders'

export const orderKeys = {
  all: ['orders'] as const,
  paged: (params: object) => ['orders', 'paged', params] as const,
  detail: (id: string) => ['orders', 'detail', id] as const,
  byCustomer: (customerId: string) => ['orders', 'customer', customerId] as const,
}

export function useOrdersPaged(params: {
  customerId?: string
  status?: string
  page?: number
  pageSize?: number
}) {
  return useQuery({
    queryKey: orderKeys.paged(params),
    queryFn: () => ordersApi.getPaged(params).then(r => r.data),
  })
}

export function useOrderById(id: string) {
  return useQuery({
    queryKey: orderKeys.detail(id),
    queryFn: () => ordersApi.getById(id).then(r => r.data),
    enabled: !!id,
  })
}

export function useOrdersByCustomer(customerId: string) {
  return useQuery({
    queryKey: orderKeys.byCustomer(customerId),
    queryFn: () => ordersApi.getByCustomer(customerId).then(r => r.data),
    enabled: !!customerId,
  })
}

export function useCreateOrder() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateOrderPayload) =>
      ordersApi.create(payload).then(r => r.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: orderKeys.all })
    },
  })
}

export function useCancelOrder() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason?: string }) =>
      ordersApi.cancel(id, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: orderKeys.all })
    },
  })
}
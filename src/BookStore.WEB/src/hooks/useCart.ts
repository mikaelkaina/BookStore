import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { cartsApi } from '../api/endpoints/carts'

export const cartKeys = {
  all: ['cart'] as const,
  byIdentifier: (customerId?: string, sessionId?: string) =>
    ['cart', customerId ?? sessionId] as const,
}

export function useCart(customerId?: string, sessionId?: string) {
  return useQuery({
    queryKey: cartKeys.byIdentifier(customerId, sessionId),
    queryFn: () => cartsApi.getCart({ customerId, sessionId }).then(r => r.data),
    enabled: !!(customerId || sessionId),
    retry: false,
  })
}

export function useAddToCart() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: {
      customerId?: string
      sessionId?: string
      bookId: string
      quantity: number
    }) => cartsApi.addItem(payload).then(r => r.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all })
    },
  })
}

export function useRemoveFromCart() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ cartId, bookId }: { cartId: string; bookId: string }) =>
      cartsApi.removeItem(cartId, bookId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all })
    },
  })
}

export function useUpdateCartQuantity() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ cartId, bookId, quantity }: {
      cartId: string
      bookId: string
      quantity: number
    }) => cartsApi.updateQuantity(cartId, bookId, quantity),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all })
    },
  })
}

export function useClearCart() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (cartId: string) => cartsApi.clear(cartId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all })
    },
  })
}

export function useCheckoutCart() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (cartId: string) => cartsApi.checkout(cartId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all })
    },
  })
}
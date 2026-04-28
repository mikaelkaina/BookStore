import api from '../axios';
import type { Cart } from '../../types';

export const cartsApi = {
  getCart: (params: { customerId?: string; sessionId?: string }) =>
    api.get<Cart>('/carts', { params }),

  addItem: (payload: { customerId?: string; sessionId?: string; bookId: string; quantity: number }) =>
    api.post<Cart>('/carts/items', payload),

  removeItem: (cartId: string, bookId: string) =>
    api.delete(`/carts/${cartId}/items/${bookId}`),

  updateQuantity: (cartId: string, bookId: string, quantity: number) =>
    api.patch(`/carts/${cartId}/items/${bookId}`, { cartId, bookId, quantity }),

  clear: (cartId: string) =>
    api.delete(`/carts/${cartId}`),

  checkout: (cartId: string) =>
    api.post(`/carts/${cartId}/checkout`),
};
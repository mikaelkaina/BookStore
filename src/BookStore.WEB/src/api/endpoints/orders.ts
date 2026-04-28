import api from '../axios';
import type { Order, OrderSummary, PagedResponse } from '../../types';

export interface CreateOrderPayload {
  customerId: string;
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  state: string;
  zipCode: string;
  notes?: string;
  items: { bookId: string; quantity: number }[];
}

export const ordersApi = {
  getPaged: (params: { customerId?: string; status?: string; page?: number; pageSize?: number }) =>
    api.get<PagedResponse<OrderSummary>>('/orders', { params }),

  getById: (id: string) =>
    api.get<Order>(`/orders/${id}`),

  getByCustomer: (customerId: string) =>
    api.get<OrderSummary[]>(`/orders/customer/${customerId}`),

  create: (payload: CreateOrderPayload) =>
    api.post<Order>('/orders', payload),

  confirmPayment: (id: string) =>
    api.patch(`/orders/${id}/payment/confirm`),

  startProcessing: (id: string) =>
    api.patch(`/orders/${id}/processing/start`),

  ship: (id: string) =>
    api.patch(`/orders/${id}/ship`),

  deliver: (id: string) =>
    api.patch(`/orders/${id}/deliver`),

  cancel: (id: string, reason?: string) =>
    api.patch(`/orders/${id}/cancel`, { orderId: id, reason }),

  applyDiscount: (id: string, discountAmount: number) =>
    api.patch(`/orders/${id}/discount`, { orderId: id, discountAmount }),

  setShipping: (id: string, shippingCost: number) =>
    api.patch(`/orders/${id}/shipping`, { orderId: id, shippingCost }),
};
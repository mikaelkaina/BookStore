import api from '../axios';
import type { Customer } from '../../types';

export interface CreateCustomerPayload {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  document: string;
  birthDate?: string;
}

export const customersApi = {
  getById: (id: string) =>
    api.get<Customer>(`/customers/${id}`),

  getByEmail: (email: string) =>
    api.get<Customer>(`/customers/email/${email}`),

  register: (payload: CreateCustomerPayload) =>
    api.post<Customer>('/customers', payload),

  updateProfile: (id: string, payload: { firstName: string; lastName: string; phone?: string }) =>
    api.put<Customer>(`/customers/${id}`, { customerId: id, ...payload }),

  delete: (id: string) =>
    api.delete(`/customers/${id}`),
};
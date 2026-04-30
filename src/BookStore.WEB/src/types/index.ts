// ─── Books ────────────────────────────────────────────────────────────────────

export interface BookDetail {
  id: string;
  title: string;
  author: string;
  description?: string;
  isbn: string;
  price: number;
  currency: string;
  stockQuantity: number;
  pageCount: number;
  coverImageUrl?: string;
  publisher: string;
  publishedDate: string;
  format: string;
  language: string;
  isActive: boolean;
  categoryId: string;
  categoryName: string;
  createdAt: string;
  updatedAt?: string;
}

export interface BookSummary {
  id: string;
  title: string;
  author: string;
  isbn: string;
  price: number;
  currency: string;
  stockQuantity: number;
  coverImageUrl?: string;
  format: string;
  categoryName: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// ─── Categories ───────────────────────────────────────────────────────────────

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  isActive: boolean;
}

// ─── Customers ────────────────────────────────────────────────────────────────

export interface Customer {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone?: string;
  document: string;
  birthDate?: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

// ─── Orders ───────────────────────────────────────────────────────────────────

export interface OrderItem {
  bookId: string;
  bookTitle: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  currency: string;
}

export interface ShippingAddress {
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  state: string;
  zipCode: string;
}

export interface Order {
  id: string;
  orderNumber: string;
  customerId: string;
  status: string;
  subTotal: number;
  shippingCost: number;
  discount: number;
  total: number;
  currency: string;
  shippingAddress: ShippingAddress;
  items: OrderItem[];
  notes?: string;
  shippedAt?: string;
  deliveredAt?: string;
  cancelledAt?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface OrderSummary {
  id: string;
  orderNumber: string;
  customerId: string;
  status: string;
  total: number;
  currency: string;
  itemCount: number;
  createdAt: string;
}

// ─── Cart ─────────────────────────────────────────────────────────────────────

export interface CartItem {
  bookId: string;
  bookTitle: string;
  bookCoverUrl?: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  currency: string;
}

export interface Cart {
  id: string;
  customerId?: string;
  sessionId?: string;
  items: CartItem[];
  total: number;
  currency: string;
  totalItems: number;
  isCheckedOut: boolean;
  expiresAt: string;
  createdAt: string;
  updatedAt?: string;
}


export interface AuthResponse {
  accessToken: string
  refreshToken: string
  expiresAt: string
  userId: string
  email: string
  firstName: string
  lastName: string
  customerId?: string
  roles: string[]
}

export interface AuthUser {
  userId: string
  email: string
  firstName: string
  lastName: string
  customerId?: string
  roles: string[]
}
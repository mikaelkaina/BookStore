import { Routes, Route } from 'react-router-dom'
import Layout from './components/layout/Layout'
import ProtectedRoute from './components/auth/ProtectedRoute'

import BooksPage from './pages/books/BooksPage'
import BookDetailPage from './pages/books/BookDetailPage'
import BookFormPage from './pages/books/BookFormPage'
import CategoriesPage from './pages/categories/CategoriesPage'
import CartPage from './pages/carts/CartPage'
import CheckoutPage from './pages/carts/CheckoutPage'
import OrdersPage from './pages/orders/OrdersPage'
import OrderDetailPage from './pages/orders/OrderDetailPage'
import LoginPage from './pages/auth/LoginPage'
import RegisterPage from './pages/auth/RegisterPage'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>

        <Route index element={<BooksPage />} />
        <Route path="books" element={<BooksPage />} />
        <Route path="books/:id" element={<BookDetailPage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />

        <Route path="categories" element={
          <ProtectedRoute requireAdmin>
            <CategoriesPage />
          </ProtectedRoute>
        } />
        <Route path="cart" element={
          <ProtectedRoute>
            <CartPage />
          </ProtectedRoute>
        } />
        <Route path="checkout" element={
          <ProtectedRoute>
            <CheckoutPage />
          </ProtectedRoute>
        } />
        <Route path="orders" element={
          <ProtectedRoute>
            <OrdersPage />
          </ProtectedRoute>
        } />
        <Route path="orders/:id" element={
          <ProtectedRoute>
            <OrderDetailPage />
          </ProtectedRoute>
        } />

        <Route path="books/new" element={
          <ProtectedRoute requireAdmin>
            <BookFormPage />
          </ProtectedRoute>
        } />

      </Route>
    </Routes>
  )
}
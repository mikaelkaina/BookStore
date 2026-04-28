import { Routes, Route } from 'react-router-dom'
import Layout from './components/layout/Layout'

// Pages — vamos criar em seguida
import HomePage from './pages/HomePage'
import BooksPage from './pages/books/BooksPage'
import BookDetailPage from './pages/books/BookDetailPage'
import CategoriesPage from './pages/categories/CategoriesPage'
import CartPage from './pages/carts/CartPage'
import OrdersPage from './pages/orders/OrdersPage'
import OrderDetailPage from './pages/orders/OrderDetailPage'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="books" element={<BooksPage />} />
        <Route path="books/:id" element={<BookDetailPage />} />
        <Route path="categories" element={<CategoriesPage />} />
        <Route path="cart" element={<CartPage />} />
        <Route path="orders" element={<OrdersPage />} />
        <Route path="orders/:id" element={<OrderDetailPage />} />
      </Route>
    </Routes>
  )
}
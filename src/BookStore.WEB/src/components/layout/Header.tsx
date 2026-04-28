import { Link, NavLink } from 'react-router-dom'
import { BookOpen, ShoppingCart, ClipboardList, LayoutGrid } from 'lucide-react'

export default function Header() {
  const navLinkClass = ({ isActive }: { isActive: boolean }) =>
    `flex items-center gap-2 px-3 py-2 rounded-md text-sm font-medium transition-colors ${
      isActive
        ? 'bg-indigo-700 text-white'
        : 'text-indigo-100 hover:bg-indigo-700 hover:text-white'
    }`

  return (
    <header className="bg-indigo-600 shadow-md">
      <div className="container mx-auto px-4 max-w-7xl">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link to="/" className="flex items-center gap-2 text-white font-bold text-xl">
            <BookOpen size={28} />
            BookStore
          </Link>

          {/* Nav */}
          <nav className="flex items-center gap-1">
            <NavLink to="/books" className={navLinkClass}>
              <BookOpen size={16} />
              Livros
            </NavLink>

            <NavLink to="/categories" className={navLinkClass}>
              <LayoutGrid size={16} />
              Categorias
            </NavLink>

            <NavLink to="/orders" className={navLinkClass}>
              <ClipboardList size={16} />
              Pedidos
            </NavLink>

            <NavLink to="/cart" className={navLinkClass}>
              <ShoppingCart size={16} />
              Carrinho
            </NavLink>
          </nav>
        </div>
      </div>
    </header>
  )
}
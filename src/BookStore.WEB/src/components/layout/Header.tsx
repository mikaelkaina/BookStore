import { Link, NavLink, useNavigate } from 'react-router-dom'
import {
  BookOpen, ShoppingCart, ClipboardList,
  LayoutGrid, LogIn, LogOut, UserCircle, UserPlus
} from 'lucide-react'
import { useAuth } from '../../contexts/AuthContext'

export default function Header() {
  const { user, isAuthenticated, isAdmin, logout } = useAuth()
  const navigate = useNavigate()

  const navLinkClass = ({ isActive }: { isActive: boolean }) =>
    `flex items-center gap-2 px-3 py-2 rounded-md text-sm font-medium transition-colors ${
      isActive
        ? 'bg-indigo-700 text-white'
        : 'text-indigo-100 hover:bg-indigo-700 hover:text-white'
    }`

  function handleLogout() {
    logout()
    navigate('/')
  }

  return (
    <header className="bg-indigo-600 shadow-md">
      <div className="container mx-auto px-4 max-w-7xl">
        <div className="flex items-center justify-between h-16">

          {/* Logo */}
          <Link to="/" className="flex items-center gap-2 text-white font-bold text-xl">
            <BookOpen size={28} />
            BookStore
          </Link>

          {/* Nav central */}
          <nav className="flex items-center gap-1">
            <NavLink to="/books" className={navLinkClass}>
              <BookOpen size={16} />
              Livros
            </NavLink>

            {isAdmin && (
              <NavLink to="/categories" className={navLinkClass}>
                <LayoutGrid size={16} />
                Categorias
              </NavLink>
            )}

            {isAuthenticated && (
              <>
                <NavLink to="/orders" className={navLinkClass}>
                  <ClipboardList size={16} />
                  Pedidos
                </NavLink>

                <NavLink to="/cart" className={navLinkClass}>
                  <ShoppingCart size={16} />
                  Carrinho
                </NavLink>
              </>
            )}
          </nav>

          {/* Auth */}
          <div className="flex items-center gap-2">
            {isAuthenticated ? (
              <>
                <div className="flex items-center gap-2 text-indigo-100 text-sm px-3">
                  <UserCircle size={20} />
                  <span className="hidden md:block">
                    {user?.firstName} {user?.lastName}
                    {isAdmin && (
                      <span className="ml-1 text-xs bg-indigo-800 px-1.5 py-0.5 rounded-full">
                        Admin
                      </span>
                    )}
                  </span>
                </div>
                <button
                  onClick={handleLogout}
                  className="flex items-center gap-2 px-3 py-2 rounded-md text-sm font-medium text-indigo-100 hover:bg-indigo-700 transition-colors"
                >
                  <LogOut size={16} />
                  <span className="hidden md:block">Sair</span>
                </button>
              </>
            ) : (
              <>
                <NavLink to="/login" className={navLinkClass}>
                  <LogIn size={16} />
                  <span className="hidden md:block">Entrar</span>
                </NavLink>
                <NavLink
                  to="/register"
                  className="flex items-center gap-2 px-3 py-2 rounded-md text-sm font-medium bg-white text-indigo-600 hover:bg-indigo-50 transition-colors"
                >
                  <UserPlus size={16} />
                  <span className="hidden md:block">Cadastrar</span>
                </NavLink>
              </>
            )}
          </div>

        </div>
      </div>
    </header>
  )
}
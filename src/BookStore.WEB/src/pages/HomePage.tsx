import { Link } from 'react-router-dom'
import { BookOpen, LayoutGrid, ShoppingCart } from 'lucide-react'

export default function HomePage() {
  return (
    <div className="flex flex-col items-center justify-center gap-8 py-16">
      <h1 className="text-4xl font-bold text-gray-800">
        Bem-vindo à BookStore
      </h1>
      <p className="text-gray-500 text-lg text-center max-w-xl">
        Encontre os melhores livros, adicione ao carrinho e faça seu pedido com facilidade.
      </p>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-4 w-full max-w-3xl">
        <Link
          to="/books"
          className="flex flex-col items-center gap-3 p-6 bg-white rounded-xl shadow hover:shadow-md border border-gray-100 transition-all hover:-translate-y-1"
        >
          <BookOpen size={36} className="text-indigo-600" />
          <span className="font-semibold text-gray-700">Ver Livros</span>
        </Link>

        <Link
          to="/categories"
          className="flex flex-col items-center gap-3 p-6 bg-white rounded-xl shadow hover:shadow-md border border-gray-100 transition-all hover:-translate-y-1"
        >
          <LayoutGrid size={36} className="text-indigo-600" />
          <span className="font-semibold text-gray-700">Categorias</span>
        </Link>

        <Link
          to="/cart"
          className="flex flex-col items-center gap-3 p-6 bg-white rounded-xl shadow hover:shadow-md border border-gray-100 transition-all hover:-translate-y-1"
        >
          <ShoppingCart size={36} className="text-indigo-600" />
          <span className="font-semibold text-gray-700">Carrinho</span>
        </Link>
      </div>
    </div>
  )
}
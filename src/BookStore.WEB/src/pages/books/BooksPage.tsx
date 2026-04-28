import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Search, SlidersHorizontal, Plus } from 'lucide-react'
import { useBooksPaged } from '../../hooks/useBooks'
import { useCategories } from '../../hooks/useCategories'
import Spinner from '../../components/ui/Spinner'
import ErrorMessage from '../../components/ui/ErrorMessage'
import EmptyState from '../../components/ui/EmptyState'
import Button from '../../components/ui/Button'
import Badge from '../../components/ui/Badge'

export default function BooksPage() {
  const [searchTerm, setSearchTerm] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [minPrice, setMinPrice] = useState('')
  const [maxPrice, setMaxPrice] = useState('')
  const [sortByPrice, setSortByPrice] = useState(false)
  const [ascending, setAscending] = useState(true)
  const [page, setPage] = useState(1)

  const { data, isLoading, isError } = useBooksPaged({
    searchTerm: searchTerm || undefined,
    categoryId: categoryId || undefined,
    minPrice: minPrice ? Number(minPrice) : undefined,
    maxPrice: maxPrice ? Number(maxPrice) : undefined,
    sortByPrice: sortByPrice || undefined,
    ascending,
    page,
    pageSize: 12,
  })

  const { data: categories } = useCategories()

  function handleSearch(e: React.FormEvent) {
    e.preventDefault()
    setPage(1)
  }

  function handleReset() {
    setSearchTerm('')
    setCategoryId('')
    setMinPrice('')
    setMaxPrice('')
    setSortByPrice(false)
    setAscending(true)
    setPage(1)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Livros</h1>
        <Link to="/books/new">
          <Button>
            <Plus size={16} />
            Novo Livro
          </Button>
        </Link>
      </div>

      <form
        onSubmit={handleSearch}
        className="bg-white rounded-xl shadow-sm border border-gray-100 p-4 mb-6"
      >
        <div className="flex items-center gap-2 mb-3">
          <SlidersHorizontal size={16} className="text-gray-500" />
          <span className="text-sm font-medium text-gray-600">Filtros</span>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-3">
          <div className="relative md:col-span-2">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              placeholder="Buscar por título, autor..."
              value={searchTerm}
              onChange={e => setSearchTerm(e.target.value)}
              className="w-full pl-9 pr-4 py-2 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>

          <select
            value={categoryId}
            onChange={e => { setCategoryId(e.target.value); setPage(1) }}
            className="border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          >
            <option value="">Todas as categorias</option>
            {categories?.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>

          <select
            value={`${sortByPrice}-${ascending}`}
            onChange={e => {
              const [byPrice, asc] = e.target.value.split('-')
              setSortByPrice(byPrice === 'true')
              setAscending(asc === 'true')
              setPage(1)
            }}
            className="border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          >
            <option value="false-true">Título A-Z</option>
            <option value="false-false">Título Z-A</option>
            <option value="true-true">Menor preço</option>
            <option value="true-false">Maior preço</option>
          </select>

          <input
            type="number"
            placeholder="Preço mínimo"
            value={minPrice}
            onChange={e => setMinPrice(e.target.value)}
            className="border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />
          <input
            type="number"
            placeholder="Preço máximo"
            value={maxPrice}
            onChange={e => setMaxPrice(e.target.value)}
            className="border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />

          <div className="flex gap-2 md:col-span-2">
            <Button type="submit" className="flex-1">
              <Search size={14} />
              Buscar
            </Button>
            <Button type="button" variant="secondary" onClick={handleReset}>
              Limpar
            </Button>
          </div>
        </div>
      </form>

      {isLoading && <Spinner />}
      {isError && <ErrorMessage message="Não foi possível carregar os livros." />}

      {data && data.items.length === 0 && (
        <EmptyState message="Nenhum livro encontrado." />
      )}

      {data && data.items.length > 0 && (
        <>
          <p className="text-sm text-gray-500 mb-4">
            {data.totalCount} livro{data.totalCount !== 1 ? 's' : ''} encontrado{data.totalCount !== 1 ? 's' : ''}
          </p>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-5">
            {data.items.map(book => (
              <Link
                key={book.id}
                to={`/books/${book.id}`}
                className="bg-white rounded-xl shadow-sm border border-gray-100 hover:shadow-md transition-all hover:-translate-y-1 flex flex-col overflow-hidden"
              >
                <div className="bg-indigo-50 h-48 flex items-center justify-center">
                  {book.coverImageUrl ? (
                    <img
                      src={book.coverImageUrl}
                      alt={book.title}
                      className="h-full w-full object-cover"
                    />
                  ) : (
                    <span className="text-indigo-200 text-5xl">📚</span>
                  )}
                </div>

                <div className="p-4 flex flex-col gap-2 flex-1">
                  <Badge label={book.categoryName} variant="info" />
                  <h3 className="font-semibold text-gray-800 text-sm leading-tight line-clamp-2">
                    {book.title}
                  </h3>
                  <p className="text-xs text-gray-500">{book.author}</p>

                  <div className="mt-auto flex items-center justify-between pt-2">
                    <span className="text-indigo-600 font-bold text-base">
                      {book.price.toLocaleString('pt-BR', {
                        style: 'currency',
                        currency: book.currency,
                      })}
                    </span>
                    <span className={`text-xs ${book.stockQuantity > 0 ? 'text-green-600' : 'text-red-500'}`}>
                      {book.stockQuantity > 0 ? `${book.stockQuantity} em estoque` : 'Esgotado'}
                    </span>
                  </div>
                </div>
              </Link>
            ))}
          </div>

          <div className="flex items-center justify-center gap-2 mt-8">
            <Button
              variant="secondary"
              size="sm"
              disabled={!data.hasPreviousPage}
              onClick={() => setPage(p => p - 1)}
            >
              Anterior
            </Button>

            <span className="text-sm text-gray-600 px-3">
              Página {data.page} de {data.totalPages}
            </span>

            <Button
              variant="secondary"
              size="sm"
              disabled={!data.hasNextPage}
              onClick={() => setPage(p => p + 1)}
            >
              Próxima
            </Button>
          </div>
        </>
      )}
    </div>
  )
}
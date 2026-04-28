import { useState } from 'react'
import { useParams, useNavigate, Link } from 'react-router-dom'
import { ArrowLeft, ShoppingCart, Package, BookOpen, Calendar, Hash } from 'lucide-react'
import { useBookById } from '../../hooks/useBooks'
import { useAddToCart } from '../../hooks/useCart'
import Spinner from '../../components/ui/Spinner'
import ErrorMessage from '../../components/ui/ErrorMessage'
import Button from '../../components/ui/Button'
import Badge from '../../components/ui/Badge'

function getSessionId(): string {
  let sessionId = localStorage.getItem('bookstore_session')
  if (!sessionId) {
    sessionId = crypto.randomUUID()
    localStorage.setItem('bookstore_session', sessionId)
  }
  return sessionId
}

export default function BookDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [quantity, setQuantity] = useState(1)
  const [addedToCart, setAddedToCart] = useState(false)
  const [cartError, setCartError] = useState('')

  const { data: book, isLoading, isError } = useBookById(id!)
  const addToCart = useAddToCart()

  async function handleAddToCart() {
    if (!book) return
    setCartError('')

    try {
      await addToCart.mutateAsync({
        sessionId: getSessionId(),
        bookId: book.id,
        quantity,
      })
      setAddedToCart(true)
      setTimeout(() => setAddedToCart(false), 3000)
    } catch {
      setCartError('Erro ao adicionar ao carrinho. Tente novamente.')
    }
  }

  if (isLoading) return <Spinner />
  if (isError || !book) return <ErrorMessage message="Livro não encontrado." />

  return (
    <div className="max-w-5xl mx-auto">
      <button
        onClick={() => navigate('/books')}
        className="flex items-center gap-2 text-gray-500 hover:text-gray-700 mb-6 transition-colors"
      >
        <ArrowLeft size={18} />
        <span className="text-sm">Voltar para Livros</span>
      </button>

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-0">

          <div className="bg-indigo-50 flex items-center justify-center p-8 min-h-72">
            {book.coverImageUrl ? (
              <img
                src={book.coverImageUrl}
                alt={book.title}
                className="max-h-80 object-contain rounded-lg shadow-md"
              />
            ) : (
              <span className="text-8xl">📚</span>
            )}
          </div>

          <div className="md:col-span-2 p-8 flex flex-col gap-4">
            <div className="flex items-start justify-between gap-4">
              <div>
                <Badge label={book.categoryName} variant="info" />
                <h1 className="text-2xl font-bold text-gray-900 mt-2 leading-tight">
                  {book.title}
                </h1>
                <p className="text-gray-500 mt-1">{book.author}</p>
              </div>
              <Badge
                label={book.isActive ? 'Ativo' : 'Inativo'}
                variant={book.isActive ? 'success' : 'danger'}
              />
            </div>

            <div className="flex items-baseline gap-2">
              <span className="text-3xl font-bold text-indigo-600">
                {book.price.toLocaleString('pt-BR', {
                  style: 'currency',
                  currency: book.currency,
                })}
              </span>
            </div>

            {book.description && (
              <p className="text-gray-600 text-sm leading-relaxed border-t pt-4">
                {book.description}
              </p>
            )}

            <div className="grid grid-cols-2 gap-3 border-t pt-4">
              <div className="flex items-center gap-2 text-sm text-gray-500">
                <Hash size={14} className="text-indigo-400" />
                <span>ISBN: {book.isbn}</span>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-500">
                <BookOpen size={14} className="text-indigo-400" />
                <span>{book.pageCount} páginas</span>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-500">
                <Package size={14} className="text-indigo-400" />
                <span>Editora: {book.publisher}</span>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-500">
                <Calendar size={14} className="text-indigo-400" />
                <span>
                  {new Date(book.publishedDate).toLocaleDateString('pt-BR')}
                </span>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-500">
                <span className="text-indigo-400 font-bold text-xs">PT</span>
                <span>Idioma: {book.language}</span>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-500">
                <span className="text-indigo-400 font-bold text-xs">📦</span>
                <span>Formato: {book.format}</span>
              </div>
            </div>

            <div className="border-t pt-4">
              {book.stockQuantity > 0 ? (
                <>
                  <p className="text-green-600 text-sm font-medium mb-3">
                    ✓ {book.stockQuantity} unidade{book.stockQuantity !== 1 ? 's' : ''} em estoque
                  </p>

                  <div className="flex items-center gap-3">
                    <div className="flex items-center border border-gray-200 rounded-lg overflow-hidden">
                      <button
                        onClick={() => setQuantity(q => Math.max(1, q - 1))}
                        className="px-3 py-2 text-gray-500 hover:bg-gray-50 transition-colors font-bold"
                      >
                        −
                      </button>
                      <span className="px-4 py-2 text-sm font-medium text-gray-700 min-w-[3rem] text-center">
                        {quantity}
                      </span>
                      <button
                        onClick={() => setQuantity(q => Math.min(book.stockQuantity, q + 1))}
                        className="px-3 py-2 text-gray-500 hover:bg-gray-50 transition-colors font-bold"
                      >
                        +
                      </button>
                    </div>

                    <Button
                      onClick={handleAddToCart}
                      loading={addToCart.isPending}
                      size="lg"
                      className="flex-1"
                    >
                      <ShoppingCart size={18} />
                      {addedToCart ? '✓ Adicionado!' : 'Adicionar ao Carrinho'}
                    </Button>
                  </div>

                  {cartError && (
                    <p className="text-red-500 text-sm mt-2">{cartError}</p>
                  )}

                  {addedToCart && (
                    <div className="mt-3 flex items-center gap-2">
                      <p className="text-green-600 text-sm">
                        Item adicionado ao carrinho!
                      </p>
                      <Link
                        to="/cart"
                        className="text-sm text-indigo-600 underline hover:text-indigo-800"
                      >
                        Ver carrinho →
                      </Link>
                    </div>
                  )}
                </>
              ) : (
                <p className="text-red-500 text-sm font-medium">
                  ✗ Produto esgotado
                </p>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
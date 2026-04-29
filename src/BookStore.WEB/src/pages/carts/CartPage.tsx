//import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Trash2, ShoppingCart, ArrowRight, BookOpen } from 'lucide-react'
import {
  useCart,
  useRemoveFromCart,
  useUpdateCartQuantity,
  useClearCart,
} from '../../hooks/useCart'
import Spinner from '../../components/ui/Spinner'
import Button from '../../components/ui/Button'
import EmptyState from '../../components/ui/EmptyState'

function getSessionId(): string {
  let sessionId = localStorage.getItem('bookstore_session')
  if (!sessionId) {
    sessionId = crypto.randomUUID()
    localStorage.setItem('bookstore_session', sessionId)
  }
  return sessionId
}

export default function CartPage() {
  const sessionId = getSessionId()

  const { data: cart, isLoading } = useCart(undefined, sessionId)
  const removeItem = useRemoveFromCart()
  const updateQuantity = useUpdateCartQuantity()
  const clearCart = useClearCart()

  async function handleRemove(bookId: string) {
    if (!cart) return
    await removeItem.mutateAsync({ cartId: cart.id, bookId })
  }

  async function handleQuantityChange(bookId: string, quantity: number) {
    if (!cart) return
    await updateQuantity.mutateAsync({ cartId: cart.id, bookId, quantity })
  }

  async function handleClear() {
    if (!cart) return
    if (!confirm('Deseja limpar o carrinho?')) return
    await clearCart.mutateAsync(cart.id)
  }

  if (isLoading) return <Spinner />

  const isEmpty = !cart || cart.items.length === 0

  return (
    <div className="max-w-4xl mx-auto">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800 flex items-center gap-2">
          <ShoppingCart size={24} className="text-indigo-600" />
          Carrinho
        </h1>
        {!isEmpty && (
          <Button variant="ghost" size="sm" onClick={handleClear}>
            <Trash2 size={14} />
            Limpar carrinho
          </Button>
        )}
      </div>

      {isEmpty ? (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 py-16">
          <EmptyState message="Seu carrinho está vazio." />
          <div className="flex justify-center mt-4">
            <Link to="/books">
              <Button variant="secondary">
                <BookOpen size={16} />
                Ver Livros
              </Button>
            </Link>
          </div>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

          <div className="lg:col-span-2 flex flex-col gap-3">
            {cart.items.map(item => (
              <div
                key={item.bookId}
                className="bg-white rounded-xl shadow-sm border border-gray-100 p-4 flex gap-4"
              >
                <div className="w-16 h-20 bg-indigo-50 rounded-lg flex items-center justify-center shrink-0 overflow-hidden">
                  {item.bookCoverUrl ? (
                    <img
                      src={item.bookCoverUrl}
                      alt={item.bookTitle}
                      className="w-full h-full object-cover"
                    />
                  ) : (
                    <span className="text-2xl">📚</span>
                  )}
                </div>

                <div className="flex-1 min-w-0">
                  <h3 className="font-semibold text-gray-800 text-sm leading-tight truncate">
                    {item.bookTitle}
                  </h3>
                  <p className="text-indigo-600 font-bold mt-1">
                    {item.unitPrice.toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: item.currency,
                    })}
                  </p>

                  <div className="flex items-center gap-2 mt-2">
                    <div className="flex items-center border border-gray-200 rounded-lg overflow-hidden">
                      <button
                        onClick={() => handleQuantityChange(item.bookId, item.quantity - 1)}
                        className="px-2 py-1 text-gray-500 hover:bg-gray-50 transition-colors text-sm font-bold"
                      >
                        −
                      </button>
                      <span className="px-3 py-1 text-sm font-medium text-gray-700 min-w-[2rem] text-center">
                        {item.quantity}
                      </span>
                      <button
                        onClick={() => handleQuantityChange(item.bookId, item.quantity + 1)}
                        className="px-2 py-1 text-gray-500 hover:bg-gray-50 transition-colors text-sm font-bold"
                      >
                        +
                      </button>
                    </div>

                    <span className="text-sm text-gray-500">
                      = {item.totalPrice.toLocaleString('pt-BR', {
                        style: 'currency',
                        currency: item.currency,
                      })}
                    </span>
                  </div>
                </div>

                <button
                  onClick={() => handleRemove(item.bookId)}
                  className="p-2 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors h-fit"
                >
                  <Trash2 size={16} />
                </button>
              </div>
            ))}
          </div>

          <div className="lg:col-span-1">
            <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 sticky top-4">
              <h2 className="font-semibold text-gray-800 mb-4">Resumo do Pedido</h2>

              <div className="flex flex-col gap-2 text-sm">
                <div className="flex justify-between text-gray-600">
                  <span>{cart.totalItems} item{cart.totalItems !== 1 ? 's' : ''}</span>
                  <span>
                    {cart.total.toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: cart.currency,
                    })}
                  </span>
                </div>

                <div className="flex justify-between text-gray-600">
                  <span>Frete</span>
                  <span className="text-green-600">A calcular</span>
                </div>

                <div className="border-t pt-2 mt-1 flex justify-between font-bold text-gray-800 text-base">
                  <span>Total</span>
                  <span className="text-indigo-600">
                    {cart.total.toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: cart.currency,
                    })}
                  </span>
                </div>
              </div>

              <Link to="/checkout">
                <Button className="w-full mt-4" size="lg">
                  Finalizar Compra
                  <ArrowRight size={16} />
                </Button>
              </Link>

              <Link to="/books">
                <Button variant="ghost" className="w-full mt-2" size="sm">
                  Continuar Comprando
                </Button>
              </Link>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
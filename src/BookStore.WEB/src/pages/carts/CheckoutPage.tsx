import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { ArrowLeft, ShoppingBag } from 'lucide-react'
import { useCart, useClearCart } from '../../hooks/useCart'
import { useCreateOrder } from '../../hooks/useOrders'
import { useAuth } from '../../contexts/AuthContext'
import Button from '../../components/ui/Button'
import Spinner from '../../components/ui/Spinner'
import EmptyState from '../../components/ui/EmptyState'

function getSessionId(): string {
  let sessionId = localStorage.getItem('bookstore_session')
  if (!sessionId) {
    sessionId = crypto.randomUUID()
    localStorage.setItem('bookstore_session', sessionId)
  }
  return sessionId
}

export default function CheckoutPage() {
  const navigate = useNavigate()
  const sessionId = getSessionId()
  const { user } = useAuth()

  const { data: cart, isLoading } = useCart(undefined, sessionId)
  const createOrder = useCreateOrder()
  const clearCart = useClearCart()

  const [formError, setFormError] = useState('')
  const [address, setAddress] = useState({
    street: '',
    number: '',
    complement: '',
    neighborhood: '',
    city: '',
    state: '',
    zipCode: '',
  })

  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    const { name, value } = e.target
    setAddress(v => ({ ...v, [name]: value }))
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setFormError('')

    if (!user?.customerId) {
      setFormError('Usuário não identificado. Faça login novamente.')
      return
    }

    if (!cart || cart.items.length === 0) {
      setFormError('Carrinho vazio.')
      return
    }

    try {
      const order = await createOrder.mutateAsync({
        customerId: user.customerId,
        street: address.street.trim(),
        number: address.number.trim(),
        complement: address.complement.trim() || undefined,
        neighborhood: address.neighborhood.trim(),
        city: address.city.trim(),
        state: address.state.trim().toUpperCase(),
        zipCode: address.zipCode.replace(/\D/g, ''),
        items: cart.items.map(i => ({
          bookId: i.bookId,
          quantity: i.quantity,
        })),
      })

      await clearCart.mutateAsync(cart.id)
      localStorage.removeItem('bookstore_session')

      navigate(`/orders/${order.id}`)
    } catch {
      setFormError('Erro ao criar pedido. Verifique os dados e tente novamente.')
    }
  }

  if (isLoading) return <Spinner />

  if (!cart || cart.items.length === 0) {
    return (
      <div className="max-w-2xl mx-auto">
        <EmptyState message="Seu carrinho está vazio." />
      </div>
    )
  }

  return (
    <div className="max-w-4xl mx-auto">
      <button
        onClick={() => navigate('/cart')}
        className="flex items-center gap-2 text-gray-500 hover:text-gray-700 mb-6 transition-colors"
      >
        <ArrowLeft size={18} />
        <span className="text-sm">Voltar ao Carrinho</span>
      </button>

      <h1 className="text-2xl font-bold text-gray-800 mb-6 flex items-center gap-2">
        <ShoppingBag size={24} className="text-indigo-600" />
        Finalizar Compra
      </h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

        <form onSubmit={handleSubmit} className="lg:col-span-2 flex flex-col gap-5">

          <div className="bg-indigo-50 border border-indigo-100 rounded-xl p-4 flex items-center gap-3">
            <div className="bg-indigo-600 text-white rounded-full w-10 h-10 flex items-center justify-center font-bold text-sm shrink-0">
              {user?.firstName?.charAt(0)}{user?.lastName?.charAt(0)}
            </div>
            <div>
              <p className="font-medium text-gray-800 text-sm">
                {user?.firstName} {user?.lastName}
              </p>
              <p className="text-xs text-gray-500">{user?.email}</p>
            </div>
          </div>

          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
            <h2 className="font-semibold text-gray-700 mb-4">Endereço de Entrega</h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Rua <span className="text-red-500">*</span>
                </label>
                <input
                  name="street"
                  required
                  value={address.street}
                  onChange={handleChange}
                  placeholder="Ex: Rua das Flores"
                  className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Número <span className="text-red-500">*</span>
                </label>
                <input
                  name="number"
                  required
                  value={address.number}
                  onChange={handleChange}
                  placeholder="Ex: 123"
                  className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Complemento
                </label>
                <input
                  name="complement"
                  value={address.complement}
                  onChange={handleChange}
                  placeholder="Ex: Apto 42"
                  className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Bairro <span className="text-red-500">*</span>
                </label>
                <input
                  name="neighborhood"
                  required
                  value={address.neighborhood}
                  onChange={handleChange}
                  placeholder="Ex: Centro"
                  className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Cidade <span className="text-red-500">*</span>
                </label>
                <input
                  name="city"
                  required
                  value={address.city}
                  onChange={handleChange}
                  placeholder="Ex: São Paulo"
                  className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Estado (UF) <span className="text-red-500">*</span>
                </label>
                <input
                  name="state"
                  required
                  maxLength={2}
                  value={address.state}
                  onChange={handleChange}
                  placeholder="Ex: SP"
                  className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 uppercase"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  CEP <span className="text-red-500">*</span>
                </label>
                <input
                  name="zipCode"
                  required
                  value={address.zipCode}
                  onChange={handleChange}
                  placeholder="Ex: 01310-100"
                  className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>
            </div>
          </div>

          {formError && (
            <p className="text-red-500 text-sm">{formError}</p>
          )}

          <Button type="submit" size="lg" loading={createOrder.isPending}>
            Confirmar Pedido
          </Button>
        </form>

        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 h-fit sticky top-4">
          <h2 className="font-semibold text-gray-700 mb-4">Resumo</h2>

          <div className="flex flex-col gap-2 mb-4">
            {cart.items.map(item => (
              <div key={item.bookId} className="flex justify-between text-sm">
                <span className="text-gray-600 truncate flex-1 mr-2">
                  {item.bookTitle} ×{item.quantity}
                </span>
                <span className="font-medium text-gray-800 shrink-0">
                  {item.totalPrice.toLocaleString('pt-BR', {
                    style: 'currency',
                    currency: item.currency,
                  })}
                </span>
              </div>
            ))}
          </div>

          <div className="border-t pt-3 flex justify-between font-bold text-gray-800">
            <span>Total</span>
            <span className="text-indigo-600">
              {cart.total.toLocaleString('pt-BR', {
                style: 'currency',
                currency: cart.currency,
              })}
            </span>
          </div>
        </div>
      </div>
    </div>
  )
}
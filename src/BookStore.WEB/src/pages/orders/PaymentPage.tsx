import { useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, CreditCard, Lock } from 'lucide-react'
import { useOrderById } from '../../hooks/useOrders'
import Spinner from '../../components/ui/Spinner'
import ErrorMessage from '../../components/ui/ErrorMessage'
import Button from '../../components/ui/Button'

function formatCardNumber(value: string) {
  return value
    .replace(/\D/g, '')
    .slice(0, 16)
    .replace(/(.{4})/g, '$1 ')
    .trim()
}

function formatExpiry(value: string) {
  return value
    .replace(/\D/g, '')
    .slice(0, 4)
    .replace(/^(\d{2})(\d)/, '$1/$2')
}

export default function PaymentPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()

  const { data: order, isLoading, isError } = useOrderById(id!)

  const [form, setForm] = useState({
    cardNumber: '',
    cardName: '',
    expiry: '',
    cvv: '',
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    const { name, value } = e.target

    if (name === 'cardNumber') {
      setForm(v => ({ ...v, cardNumber: formatCardNumber(value) }))
      return
    }
    if (name === 'expiry') {
      setForm(v => ({ ...v, expiry: formatExpiry(value) }))
      return
    }
    if (name === 'cvv') {
      setForm(v => ({ ...v, cvv: value.replace(/\D/g, '').slice(0, 4) }))
      return
    }

    setForm(v => ({ ...v, [name]: value }))
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError('')

    if (form.cardNumber.replace(/\s/g, '').length < 16) {
      setError('Número do cartão inválido.')
      return
    }
    if (form.expiry.length < 5) {
      setError('Data de validade inválida.')
      return
    }
    if (form.cvv.length < 3) {
      setError('CVV inválido.')
      return
    }

    setLoading(true)

    await new Promise(resolve => setTimeout(resolve, 1500))

    setLoading(false)

    localStorage.setItem(`payment_sent_${id}`, 'true')

    navigate(`/orders/${id}`, {
      state: { paymentSuccess: true }
    })
  }

  if (isLoading) return <Spinner />
  if (isError || !order) return <ErrorMessage message="Pedido não encontrado." />

  return (
    <div className="max-w-4xl mx-auto">
      <button
        onClick={() => navigate(`/orders/${id}`)}
        className="flex items-center gap-2 text-gray-500 hover:text-gray-700 mb-6 transition-colors"
      >
        <ArrowLeft size={18} />
        <span className="text-sm">Voltar ao Pedido</span>
      </button>

      <h1 className="text-2xl font-bold text-gray-800 mb-6 flex items-center gap-2">
        <CreditCard size={24} className="text-indigo-600" />
        Pagamento
      </h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

        <form
          onSubmit={handleSubmit}
          className="lg:col-span-2 flex flex-col gap-5"
        >
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
            <div className="flex items-center justify-between mb-5">
              <h2 className="font-semibold text-gray-700">Dados do Cartão</h2>
              <div className="flex items-center gap-1 text-xs text-gray-400">
                <Lock size={12} />
                Pagamento seguro
              </div>
            </div>

            <div className="flex flex-col gap-4">

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Número do Cartão <span className="text-red-500">*</span>
                </label>
                <div className="relative">
                  <CreditCard
                    size={16}
                    className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"
                  />
                  <input
                    name="cardNumber"
                    required
                    value={form.cardNumber}
                    onChange={handleChange}
                    placeholder="0000 0000 0000 0000"
                    className="w-full pl-9 border border-gray-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 font-mono tracking-wider"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Nome no Cartão <span className="text-red-500">*</span>
                </label>
                <input
                  name="cardName"
                  required
                  value={form.cardName}
                  onChange={handleChange}
                  placeholder="Como aparece no cartão"
                  className="w-full border border-gray-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 uppercase"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-600 mb-1">
                    Validade <span className="text-red-500">*</span>
                  </label>
                  <input
                    name="expiry"
                    required
                    value={form.expiry}
                    onChange={handleChange}
                    placeholder="MM/AA"
                    className="w-full border border-gray-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 font-mono"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-600 mb-1">
                    CVV <span className="text-red-500">*</span>
                  </label>
                  <input
                    name="cvv"
                    required
                    type="password"
                    value={form.cvv}
                    onChange={handleChange}
                    placeholder="•••"
                    className="w-full border border-gray-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-600 mb-1">
                  Parcelas
                </label>
                <select className="w-full border border-gray-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500">
                  <option>
                    1x de {order.total.toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: order.currency,
                    })} sem juros
                  </option>
                  <option>
                    2x de {(order.total / 2).toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: order.currency,
                    })} sem juros
                  </option>
                  <option>
                    3x de {(order.total / 3).toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: order.currency,
                    })} sem juros
                  </option>
                </select>
              </div>
            </div>
          </div>

          {error && (
            <p className="text-red-500 text-sm">{error}</p>
          )}

          <Button type="submit" size="lg" loading={loading}>
            {loading ? 'Processando pagamento...' : 'Confirmar Pagamento'}
          </Button>

          <p className="text-xs text-gray-400 text-center flex items-center justify-center gap-1">
            <Lock size={10} />
            Seus dados estão protegidos com criptografia SSL
          </p>
        </form>

        <div className="flex flex-col gap-4">
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 h-fit sticky top-4">
            <h2 className="font-semibold text-gray-700 mb-4">Resumo do Pedido</h2>

            <div className="flex flex-col gap-2 text-sm mb-4">
              {order.items.map(item => (
                <div key={item.bookId} className="flex justify-between">
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

            <div className="border-t pt-3 flex flex-col gap-2 text-sm">
              <div className="flex justify-between text-gray-600">
                <span>Subtotal</span>
                <span>{order.subTotal.toLocaleString('pt-BR', {
                  style: 'currency', currency: order.currency,
                })}</span>
              </div>
              {order.shippingCost > 0 && (
                <div className="flex justify-between text-gray-600">
                  <span>Frete</span>
                  <span>{order.shippingCost.toLocaleString('pt-BR', {
                    style: 'currency', currency: order.currency,
                  })}</span>
                </div>
              )}
              {order.discount > 0 && (
                <div className="flex justify-between text-green-600">
                  <span>Desconto</span>
                  <span>- {order.discount.toLocaleString('pt-BR', {
                    style: 'currency', currency: order.currency,
                  })}</span>
                </div>
              )}
              <div className="flex justify-between font-bold text-gray-800 text-base pt-1 border-t">
                <span>Total</span>
                <span className="text-indigo-600">
                  {order.total.toLocaleString('pt-BR', {
                    style: 'currency', currency: order.currency,
                  })}
                </span>
              </div>
            </div>
          </div>

          <div className="bg-indigo-50 border border-indigo-100 rounded-xl p-4 text-center">
            <p className="text-xs text-gray-500 mb-1">Pedido</p>
            <p className="font-mono font-bold text-indigo-700 text-sm">
              {order.orderNumber}
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}
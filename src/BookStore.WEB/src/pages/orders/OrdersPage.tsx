import { useState } from 'react'
import { Link } from 'react-router-dom'
import { ClipboardList, Eye } from 'lucide-react'
import { useOrdersPaged, useOrdersByCustomer } from '../../hooks/useOrders'
import { useAuth } from '../../contexts/AuthContext'
import Spinner from '../../components/ui/Spinner'
import ErrorMessage from '../../components/ui/ErrorMessage'
import EmptyState from '../../components/ui/EmptyState'
import Badge from '../../components/ui/Badge'
import Button from '../../components/ui/Button'

const STATUS_OPTIONS = [
  { value: '', label: 'Todos os status' },
  { value: 'Pending', label: 'Pendente' },
  { value: 'PaymentConfirmed', label: 'Pagamento Confirmado' },
  { value: 'Processing', label: 'Em Processamento' },
  { value: 'Shipped', label: 'Enviado' },
  { value: 'Delivered', label: 'Entregue' },
  { value: 'Cancelled', label: 'Cancelado' },
]

function getStatusBadgeVariant(status: string) {
  switch (status) {
    case 'Pending': return 'warning'
    case 'PaymentConfirmed': return 'info'
    case 'Processing': return 'info'
    case 'Shipped': return 'default'
    case 'Delivered': return 'success'
    case 'Cancelled': return 'danger'
    default: return 'default'
  }
}

function getStatusLabel(status: string) {
  return STATUS_OPTIONS.find(s => s.value === status)?.label ?? status
}

function OrderRow({ order }: { order: any }) {
  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 flex items-center justify-between gap-4">
      <div className="flex flex-col gap-1">
        <div className="flex items-center gap-2">
          <span className="font-semibold text-gray-800 text-sm">
            {order.orderNumber}
          </span>
          <Badge
            label={getStatusLabel(order.status)}
            variant={getStatusBadgeVariant(order.status) as any}
          />
        </div>
        <span className="text-xs text-gray-500">
          {order.itemCount} item{order.itemCount !== 1 ? 's' : ''} •{' '}
          {new Date(order.createdAt).toLocaleDateString('pt-BR')}
        </span>
      </div>

      <div className="flex items-center gap-4">
        <span className="font-bold text-indigo-600">
          {order.total.toLocaleString('pt-BR', {
            style: 'currency',
            currency: order.currency,
          })}
        </span>
        <Link to={`/orders/${order.id}`}>
          <Button variant="secondary" size="sm">
            <Eye size={14} />
            Ver
          </Button>
        </Link>
      </div>
    </div>
  )
}

function AdminOrdersView() {
  const [status, setStatus] = useState('')
  const [page, setPage] = useState(1)

  const { data, isLoading, isError } = useOrdersPaged({
    status: status || undefined,
    page,
    pageSize: 10,
  })

  return (
    <>
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-4 mb-6">
        <select
          value={status}
          onChange={e => { setStatus(e.target.value); setPage(1) }}
          className="border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
        >
          {STATUS_OPTIONS.map(opt => (
            <option key={opt.value} value={opt.value}>{opt.label}</option>
          ))}
        </select>
      </div>

      {isLoading && <Spinner />}
      {isError && <ErrorMessage message="Não foi possível carregar os pedidos." />}
      {data && data.items.length === 0 && <EmptyState message="Nenhum pedido encontrado." />}

      {data && data.items.length > 0 && (
        <>
          <div className="flex flex-col gap-3">
            {data.items.map(order => <OrderRow key={order.id} order={order} />)}
          </div>

          <div className="flex items-center justify-center gap-2 mt-6">
            <Button
              variant="secondary" size="sm"
              disabled={!data.hasPreviousPage}
              onClick={() => setPage(p => p - 1)}
            >
              Anterior
            </Button>
            <span className="text-sm text-gray-600 px-3">
              Página {data.page} de {data.totalPages}
            </span>
            <Button
              variant="secondary" size="sm"
              disabled={!data.hasNextPage}
              onClick={() => setPage(p => p + 1)}
            >
              Próxima
            </Button>
          </div>
        </>
      )}
    </>
  )
}

function CustomerOrdersView({ customerId }: { customerId: string }) {
  const { data, isLoading, isError } = useOrdersByCustomer(customerId)

  if (isLoading) return <Spinner />
  if (isError) return <ErrorMessage message="Não foi possível carregar seus pedidos." />
  if (!data || data.length === 0) return <EmptyState message="Você ainda não tem pedidos." />

  return (
    <div className="flex flex-col gap-3">
      {data.map((order: any) => <OrderRow key={order.id} order={order} />)}
    </div>
  )
}

export default function OrdersPage() {
  const { user, isAdmin } = useAuth()

  return (
    <div>
      <div className="flex items-center gap-2 mb-6">
        <ClipboardList size={24} className="text-indigo-600" />
        <h1 className="text-2xl font-bold text-gray-800">
          {isAdmin ? 'Todos os Pedidos' : 'Meus Pedidos'}
        </h1>
      </div>

      {isAdmin
        ? <AdminOrdersView />
        : <CustomerOrdersView customerId={user?.customerId ?? ''} />
      }
    </div>
  )
}
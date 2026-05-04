import { useState } from 'react'
import { useParams, useNavigate, Link } from 'react-router-dom'
import { ArrowLeft, Package, CreditCard, Truck, CheckCircle, Tag } from 'lucide-react'
import { useOrderById, useCancelOrder } from '../../hooks/useOrders'
import { useAuth } from '../../contexts/AuthContext'
import { ordersApi } from '../../api/endpoints/orders'
import Spinner from '../../components/ui/Spinner'
import ErrorMessage from '../../components/ui/ErrorMessage'
import Badge from '../../components/ui/Badge'
import Button from '../../components/ui/Button'

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
    const labels: Record<string, string> = {
        Pending: 'Pendente',
        PaymentConfirmed: 'Pagamento Confirmado',
        Processing: 'Em Processamento',
        Shipped: 'Enviado',
        Delivered: 'Entregue',
        Cancelled: 'Cancelado',
    }
    return labels[status] ?? status
}

const STATUS_STEPS = [
    'Pending',
    'PaymentConfirmed',
    'Processing',
    'Shipped',
    'Delivered',
]

export default function OrderDetailPage() {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const { isAdmin } = useAuth()


    const { data: order, isLoading, isError, refetch } = useOrderById(id!)
    const cancelOrder = useCancelOrder()

    const [cancelError, setCancelError] = useState('')
    const [adminLoading, setAdminLoading] = useState<string | null>(null)
    const [adminError, setAdminError] = useState('')
    const [shippingCost, setShippingCost] = useState('')
    const [discount, setDiscount] = useState('')
    const [showShippingForm, setShowShippingForm] = useState(false)
    const [showDiscountForm, setShowDiscountForm] = useState(false)

    async function handleCancel() {
        if (!order) return
        if (!confirm('Deseja cancelar este pedido?')) return
        setCancelError('')
        try {
            await cancelOrder.mutateAsync({ id: order.id })
            navigate('/orders')
        } catch {
            setCancelError('Não foi possível cancelar o pedido.')
        }
    }

    async function handleAdminAction(action: () => Promise<any>, label: string) {
        setAdminLoading(label)
        setAdminError('')
        try {
            await action()
            await refetch()
        } catch {
            setAdminError(`Erro ao executar: ${label}`)
        } finally {
            setAdminLoading(null)
        }
    }

    async function handleSetShipping() {
        if (!order || !shippingCost) return
        await handleAdminAction(
            () => ordersApi.setShipping(order.id, Number(shippingCost)),
            'frete'
        )
        setShippingCost('')
        setShowShippingForm(false)
    }

    async function handleApplyDiscount() {
        if (!order || !discount) return
        await handleAdminAction(
            () => ordersApi.applyDiscount(order.id, Number(discount)),
            'desconto'
        )
        setDiscount('')
        setShowDiscountForm(false)
    }

    if (isLoading) return <Spinner />
    if (isError || !order) return <ErrorMessage message="Pedido não encontrado." />

    const isCancelled = order.status === 'Cancelled'
    const canCancel = !['Delivered', 'Cancelled', 'Returned'].includes(order.status)
    const currentStepIndex = STATUS_STEPS.indexOf(order.status)

    const paymentSentKey = `payment_sent_${order.id}`
    const paymentAlreadySent = !!localStorage.getItem(paymentSentKey)

    return (
        <div className="max-w-4xl mx-auto">
            <button
                onClick={() => navigate('/orders')}
                className="flex items-center gap-2 text-gray-500 hover:text-gray-700 mb-6 transition-colors"
            >
                <ArrowLeft size={18} />
                <span className="text-sm">Voltar para Pedidos</span>
            </button>

            <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 mb-4">
                <div className="flex items-start justify-between gap-4 flex-wrap">
                    <div>
                        <div className="flex items-center gap-3 flex-wrap">
                            <h1 className="text-xl font-bold text-gray-800">{order.orderNumber}</h1>
                            <Badge
                                label={getStatusLabel(order.status)}
                                variant={getStatusBadgeVariant(order.status) as any}
                            />
                        </div>
                        <p className="text-sm text-gray-500 mt-1">
                            Criado em {new Date(order.createdAt).toLocaleDateString('pt-BR')}
                            {order.shippedAt && ` • Enviado em ${new Date(order.shippedAt).toLocaleDateString('pt-BR')}`}
                            {order.deliveredAt && ` • Entregue em ${new Date(order.deliveredAt).toLocaleDateString('pt-BR')}`}
                        </p>
                    </div>

                    {canCancel && (
                        <div>
                            <Button
                                variant="danger"
                                size="sm"
                                onClick={handleCancel}
                                loading={cancelOrder.isPending}
                            >
                                Cancelar Pedido
                            </Button>
                            {cancelError && (
                                <p className="text-red-500 text-xs mt-1">{cancelError}</p>
                            )}
                        </div>
                    )}
                </div>

                {!isCancelled && (
                    <div className="mt-6">
                        <div className="flex items-center justify-between relative">
                            <div className="absolute left-0 right-0 top-3 h-0.5 bg-gray-200 z-0" />
                            <div
                                className="absolute left-0 top-3 h-0.5 bg-indigo-500 z-0 transition-all"
                                style={{
                                    width: currentStepIndex < 0
                                        ? '0%'
                                        : `${(currentStepIndex / (STATUS_STEPS.length - 1)) * 100}%`
                                }}
                            />
                            {STATUS_STEPS.map((step, index) => {
                                const isCompleted = index <= currentStepIndex
                                const isCurrent = index === currentStepIndex
                                return (
                                    <div key={step} className="flex flex-col items-center z-10 gap-1">
                                        <div className={`w-6 h-6 rounded-full border-2 flex items-center justify-center text-xs font-bold transition-all ${isCompleted
                                                ? 'bg-indigo-600 border-indigo-600 text-white'
                                                : 'bg-white border-gray-300 text-gray-400'
                                            } ${isCurrent ? 'ring-2 ring-indigo-300 ring-offset-1' : ''}`}>
                                            {isCompleted ? '✓' : index + 1}
                                        </div>
                                        <span className={`text-xs text-center max-w-[60px] leading-tight ${isCompleted ? 'text-indigo-600 font-medium' : 'text-gray-400'
                                            }`}>
                                            {getStatusLabel(step)}
                                        </span>
                                    </div>
                                )
                            })}
                        </div>
                    </div>
                )}

                {!isAdmin && order.status === 'Pending' && !paymentAlreadySent && (
                    <div className="mt-5 pt-5 border-t">
                        <Link to={`/orders/${order.id}/payment`}>
                            <Button size="lg" className="w-full">
                                <CreditCard size={16} />
                                Pagar Agora
                            </Button>
                        </Link>
                    </div>
                )}

                {!isAdmin && order.status === 'Pending' && paymentAlreadySent && (
                    <div className="mt-5 pt-5 border-t">
                        <div className="bg-yellow-50 border border-yellow-200 rounded-xl px-4 py-3 flex items-center gap-2 text-yellow-700 text-sm">
                            <CheckCircle size={16} />
                            Pagamento enviado! Aguardando confirmação do administrador.
                        </div>
                    </div>
                )}

                {isAdmin && !isCancelled && (
                    <div className="mt-5 pt-5 border-t">
                        <h3 className="text-sm font-semibold text-gray-600 mb-3">
                            Ações do Administrador
                        </h3>

                        {adminError && (
                            <p className="text-red-500 text-xs mb-3">{adminError}</p>
                        )}

                        <div className="flex flex-wrap gap-2">
                            {order.status === 'Pending' && (
                                <Button
                                    size="sm"
                                    variant="primary"
                                    loading={adminLoading === 'pagamento'}
                                    onClick={() => handleAdminAction(
                                        () => ordersApi.confirmPayment(order.id),
                                        'pagamento'
                                    )}
                                >
                                    <CreditCard size={14} />
                                    Confirmar Pagamento
                                </Button>
                            )}

                            {order.status === 'PaymentConfirmed' && (
                                <Button
                                    size="sm"
                                    variant="primary"
                                    loading={adminLoading === 'processamento'}
                                    onClick={() => handleAdminAction(
                                        () => ordersApi.startProcessing(order.id),
                                        'processamento'
                                    )}
                                >
                                    <Package size={14} />
                                    Iniciar Processamento
                                </Button>
                            )}

                            {order.status === 'Processing' && (
                                <Button
                                    size="sm"
                                    variant="primary"
                                    loading={adminLoading === 'envio'}
                                    onClick={() => handleAdminAction(
                                        () => ordersApi.ship(order.id),
                                        'envio'
                                    )}
                                >
                                    <Truck size={14} />
                                    Marcar como Enviado
                                </Button>
                            )}

                            {order.status === 'Shipped' && (
                                <Button
                                    size="sm"
                                    variant="primary"
                                    loading={adminLoading === 'entrega'}
                                    onClick={() => handleAdminAction(
                                        () => ordersApi.deliver(order.id),
                                        'entrega'
                                    )}
                                >
                                    <CheckCircle size={14} />
                                    Marcar como Entregue
                                </Button>
                            )}

                            {['Pending', 'PaymentConfirmed', 'Processing'].includes(order.status) && (
                                <Button
                                    size="sm"
                                    variant="secondary"
                                    onClick={() => {
                                        setShowShippingForm(v => !v)
                                        setShowDiscountForm(false)
                                    }}
                                >
                                    <Truck size={14} />
                                    {order.shippingCost > 0 ? 'Alterar Frete' : 'Definir Frete'}
                                </Button>
                            )}

                            {['Pending', 'PaymentConfirmed', 'Processing'].includes(order.status) && (
                                <Button
                                    size="sm"
                                    variant="secondary"
                                    onClick={() => {
                                        setShowDiscountForm(v => !v)
                                        setShowShippingForm(false)
                                    }}
                                >
                                    <Tag size={14} />
                                    Aplicar Desconto
                                </Button>
                            )}
                        </div>

                        {showShippingForm && (
                            <div className="mt-3 flex items-center gap-2">
                                <input
                                    type="number"
                                    min="0"
                                    step="0.01"
                                    value={shippingCost}
                                    onChange={e => setShippingCost(e.target.value)}
                                    placeholder="Ex: 15.90"
                                    className="border border-gray-200 rounded-lg px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 w-36"
                                />
                                <Button
                                    size="sm"
                                    loading={adminLoading === 'frete'}
                                    onClick={handleSetShipping}
                                >
                                    Confirmar
                                </Button>
                                <Button
                                    size="sm"
                                    variant="ghost"
                                    onClick={() => setShowShippingForm(false)}
                                >
                                    Cancelar
                                </Button>
                            </div>
                        )}

                        {showDiscountForm && (
                            <div className="mt-3 flex items-center gap-2">
                                <input
                                    type="number"
                                    min="0"
                                    step="0.01"
                                    value={discount}
                                    onChange={e => setDiscount(e.target.value)}
                                    placeholder="Ex: 10.00"
                                    className="border border-gray-200 rounded-lg px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 w-36"
                                />
                                <Button
                                    size="sm"
                                    loading={adminLoading === 'desconto'}
                                    onClick={handleApplyDiscount}
                                >
                                    Confirmar
                                </Button>
                                <Button
                                    size="sm"
                                    variant="ghost"
                                    onClick={() => setShowDiscountForm(false)}
                                >
                                    Cancelar
                                </Button>
                            </div>
                        )}
                    </div>
                )}
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="md:col-span-2 bg-white rounded-xl shadow-sm border border-gray-100 p-5">
                    <h2 className="font-semibold text-gray-700 mb-4 flex items-center gap-2">
                        <Package size={16} className="text-indigo-500" />
                        Itens do Pedido
                    </h2>

                    <div className="flex flex-col gap-3">
                        {order.items.map(item => (
                            <div
                                key={item.bookId}
                                className="flex items-center justify-between gap-4 py-2 border-b last:border-0"
                            >
                                <div className="flex-1 min-w-0">
                                    <p className="text-sm font-medium text-gray-800 truncate">
                                        {item.bookTitle}
                                    </p>
                                    <p className="text-xs text-gray-500 mt-0.5">
                                        {item.unitPrice.toLocaleString('pt-BR', {
                                            style: 'currency',
                                            currency: item.currency,
                                        })} × {item.quantity}
                                    </p>
                                </div>
                                <span className="text-sm font-bold text-gray-800 shrink-0">
                                    {item.totalPrice.toLocaleString('pt-BR', {
                                        style: 'currency',
                                        currency: item.currency,
                                    })}
                                </span>
                            </div>
                        ))}
                    </div>
                </div>

                <div className="flex flex-col gap-4">
                    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
                        <h2 className="font-semibold text-gray-700 mb-3">Resumo</h2>
                        <div className="flex flex-col gap-2 text-sm">
                            <div className="flex justify-between text-gray-600">
                                <span>Subtotal</span>
                                <span>{order.subTotal.toLocaleString('pt-BR', {
                                    style: 'currency', currency: order.currency,
                                })}</span>
                            </div>
                            <div className="flex justify-between text-gray-600">
                                <span>Frete</span>
                                <span>{order.shippingCost > 0
                                    ? order.shippingCost.toLocaleString('pt-BR', {
                                        style: 'currency', currency: order.currency,
                                    })
                                    : 'A calcular'
                                }</span>
                            </div>
                            {order.discount > 0 && (
                                <div className="flex justify-between text-green-600">
                                    <span>Desconto</span>
                                    <span>- {order.discount.toLocaleString('pt-BR', {
                                        style: 'currency', currency: order.currency,
                                    })}</span>
                                </div>
                            )}
                            <div className="border-t pt-2 flex justify-between font-bold text-gray-800">
                                <span>Total</span>
                                <span className="text-indigo-600">
                                    {order.total.toLocaleString('pt-BR', {
                                        style: 'currency', currency: order.currency,
                                    })}
                                </span>
                            </div>
                        </div>
                    </div>

                    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
                        <h2 className="font-semibold text-gray-700 mb-3">Endereço de Entrega</h2>
                        <div className="text-sm text-gray-600 leading-relaxed">
                            <p>
                                {order.shippingAddress.street}, {order.shippingAddress.number}
                                {order.shippingAddress.complement && `, ${order.shippingAddress.complement}`}
                            </p>
                            <p>{order.shippingAddress.neighborhood}</p>
                            <p>{order.shippingAddress.city} / {order.shippingAddress.state}</p>
                            <p>CEP: {order.shippingAddress.zipCode}</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}
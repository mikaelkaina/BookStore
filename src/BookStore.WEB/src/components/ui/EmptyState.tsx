import { PackageOpen } from 'lucide-react'

interface EmptyStateProps {
  message?: string
}

export default function EmptyState({ message = 'Nenhum item encontrado.' }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center py-16 text-gray-400">
      <PackageOpen size={48} className="mb-4" />
      <p className="text-lg">{message}</p>
    </div>
  )
}
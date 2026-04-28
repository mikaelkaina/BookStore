import { useState } from 'react'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { useCategories, useCreateCategory, useDeleteCategory } from '../../hooks/useCategories'
import Spinner from '../../components/ui/Spinner'
import ErrorMessage from '../../components/ui/ErrorMessage'
import EmptyState from '../../components/ui/EmptyState'
import Button from '../../components/ui/Button'

interface CategoryFormData {
  name: string
  description: string
}

export default function CategoriesPage() {
  const [showForm, setShowForm] = useState(false)
  const [formData, setFormData] = useState<CategoryFormData>({ name: '', description: '' })
  const [formError, setFormError] = useState('')

  const { data: categories, isLoading, isError } = useCategories()
  const createCategory = useCreateCategory()
  const deleteCategory = useDeleteCategory()

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setFormError('')

    if (!formData.name.trim()) {
      setFormError('Nome é obrigatório.')
      return
    }

    try {
      await createCategory.mutateAsync({
        name: formData.name.trim(),
        description: formData.description.trim() || undefined,
      })
      setFormData({ name: '', description: '' })
      setShowForm(false)
    } catch {
      setFormError('Erro ao criar categoria. Verifique se ela já existe.')
    }
  }

  async function handleDelete(id: string, name: string) {
    if (!confirm(`Deseja desativar a categoria "${name}"?`)) return
    try {
      await deleteCategory.mutateAsync(id)
    } catch {
      alert('Erro ao desativar categoria.')
    }
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Categorias</h1>
        <Button onClick={() => setShowForm(v => !v)}>
          <Plus size={16} />
          Nova Categoria
        </Button>
      </div>

      {showForm && (
        <form
          onSubmit={handleSubmit}
          className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 mb-6"
        >
          <h2 className="text-base font-semibold text-gray-700 mb-4">Nova Categoria</h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-600 mb-1">
                Nome <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                value={formData.name}
                onChange={e => setFormData(v => ({ ...v, name: e.target.value }))}
                placeholder="Ex: Ficção Científica"
                className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-600 mb-1">
                Descrição
              </label>
              <input
                type="text"
                value={formData.description}
                onChange={e => setFormData(v => ({ ...v, description: e.target.value }))}
                placeholder="Descrição opcional"
                className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
              />
            </div>
          </div>

          {formError && (
            <p className="text-red-500 text-sm mt-3">{formError}</p>
          )}

          <div className="flex gap-2 mt-4">
            <Button type="submit" loading={createCategory.isPending}>
              Salvar
            </Button>
            <Button
              type="button"
              variant="secondary"
              onClick={() => { setShowForm(false); setFormError('') }}
            >
              Cancelar
            </Button>
          </div>
        </form>
      )}

      {isLoading && <Spinner />}
      {isError && <ErrorMessage message="Não foi possível carregar as categorias." />}
      {!isLoading && !isError && categories?.length === 0 && (
        <EmptyState message="Nenhuma categoria cadastrada." />
      )}

      {categories && categories.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {categories.map(category => (
            <div
              key={category.id}
              className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 flex items-start justify-between"
            >
              <div>
                <h3 className="font-semibold text-gray-800">{category.name}</h3>
                {category.description && (
                  <p className="text-sm text-gray-500 mt-1">{category.description}</p>
                )}
                <p className="text-xs text-gray-400 mt-2">/{category.slug}</p>
              </div>

              <div className="flex gap-1 ml-4 shrink-0">
                <button
                  onClick={() => handleDelete(category.id, category.name)}
                  className="p-1.5 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                  title="Desativar"
                >
                  <Trash2 size={15} />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
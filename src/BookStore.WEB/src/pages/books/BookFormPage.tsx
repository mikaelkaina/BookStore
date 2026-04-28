import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { ArrowLeft } from 'lucide-react'
import { useCreateBook } from '../../hooks/useBooks'
import { useCategories } from '../../hooks/useCategories'
import Button from '../../components/ui/Button'
import Spinner from '../../components/ui/Spinner'

const FORMATS = [
  { value: 1, label: 'Brochura (Paperback)' },
  { value: 2, label: 'Capa Dura (Hardcover)' },
  { value: 3, label: 'E-book' },
  { value: 4, label: 'Audiobook' },
]

export default function BookFormPage() {
  const navigate = useNavigate()
  const createBook = useCreateBook()
  const { data: categories, isLoading: loadingCategories } = useCategories()

  const [formError, setFormError] = useState('')
  const [form, setForm] = useState({
    title: '',
    author: '',
    description: '',
    isbn: '',
    price: '',
    stockQuantity: '',
    pageCount: '',
    coverImageUrl: '',
    publisher: '',
    publishedDate: '',
    format: '1',
    language: 'Português',
    categoryId: '',
  })

  function handleChange(
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
  ) {
    const { name, value } = e.target
    setForm(v => ({ ...v, [name]: value }))
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setFormError('')

    if (!form.categoryId) {
      setFormError('Selecione uma categoria.')
      return
    }

    try {
      const created = await createBook.mutateAsync({
        title: form.title.trim(),
        author: form.author.trim(),
        description: form.description.trim() || undefined,
        isbn: form.isbn.trim(),
        price: Number(form.price),
        stockQuantity: Number(form.stockQuantity),
        pageCount: Number(form.pageCount),
        coverImageUrl: form.coverImageUrl.trim() || undefined,
        publisher: form.publisher.trim(),
        publishedDate: form.publishedDate,
        format: Number(form.format),
        language: form.language.trim(),
        categoryId: form.categoryId,
      })
      navigate(`/books/${created.id}`)
    } catch {
      setFormError('Erro ao cadastrar livro. Verifique os dados e tente novamente.')
    }
  }

  if (loadingCategories) return <Spinner />

  return (
    <div className="max-w-3xl mx-auto">
      <div className="flex items-center gap-3 mb-6">
        <button
          onClick={() => navigate('/books')}
          className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
        >
          <ArrowLeft size={20} className="text-gray-600" />
        </button>
        <h1 className="text-2xl font-bold text-gray-800">Novo Livro</h1>
      </div>

      <form
        onSubmit={handleSubmit}
        className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex flex-col gap-5"
      >
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Título <span className="text-red-500">*</span>
            </label>
            <input
              name="title"
              required
              value={form.title}
              onChange={handleChange}
              placeholder="Ex: O Senhor dos Anéis"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Autor <span className="text-red-500">*</span>
            </label>
            <input
              name="author"
              required
              value={form.author}
              onChange={handleChange}
              placeholder="Ex: J.R.R. Tolkien"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-600 mb-1">
            Descrição
          </label>
          <textarea
            name="description"
            value={form.description}
            onChange={handleChange}
            rows={3}
            placeholder="Sinopse do livro..."
            className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              ISBN <span className="text-red-500">*</span>
            </label>
            <input
              name="isbn"
              required
              value={form.isbn}
              onChange={handleChange}
              placeholder="Ex: 9788533613379"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Editora <span className="text-red-500">*</span>
            </label>
            <input
              name="publisher"
              required
              value={form.publisher}
              onChange={handleChange}
              placeholder="Ex: Martins Fontes"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Preço (R$) <span className="text-red-500">*</span>
            </label>
            <input
              name="price"
              type="number"
              required
              min="0"
              step="0.01"
              value={form.price}
              onChange={handleChange}
              placeholder="Ex: 49.90"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Estoque <span className="text-red-500">*</span>
            </label>
            <input
              name="stockQuantity"
              type="number"
              required
              min="0"
              value={form.stockQuantity}
              onChange={handleChange}
              placeholder="Ex: 50"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Nº de Páginas <span className="text-red-500">*</span>
            </label>
            <input
              name="pageCount"
              type="number"
              required
              min="1"
              value={form.pageCount}
              onChange={handleChange}
              placeholder="Ex: 520"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Formato <span className="text-red-500">*</span>
            </label>
            <select
              name="format"
              value={form.format}
              onChange={handleChange}
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              {FORMATS.map(f => (
                <option key={f.value} value={f.value}>{f.label}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Idioma <span className="text-red-500">*</span>
            </label>
            <input
              name="language"
              required
              value={form.language}
              onChange={handleChange}
              placeholder="Ex: Português"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Data de Publicação <span className="text-red-500">*</span>
            </label>
            <input
              name="publishedDate"
              type="date"
              required
              value={form.publishedDate}
              onChange={handleChange}
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Categoria <span className="text-red-500">*</span>
            </label>
            <select
              name="categoryId"
              value={form.categoryId}
              onChange={handleChange}
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">Selecione uma categoria</option>
              {categories?.map(c => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              URL da Capa
            </label>
            <input
              name="coverImageUrl"
              value={form.coverImageUrl}
              onChange={handleChange}
              placeholder="https://..."
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
        </div>

        {formError && (
          <p className="text-red-500 text-sm">{formError}</p>
        )}

        <div className="flex gap-3 pt-2">
          <Button type="submit" loading={createBook.isPending} size="lg">
            Cadastrar Livro
          </Button>
          <Button
            type="button"
            variant="secondary"
            size="lg"
            onClick={() => navigate('/books')}
          >
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  )
}
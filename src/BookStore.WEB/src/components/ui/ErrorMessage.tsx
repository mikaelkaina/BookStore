interface ErrorMessageProps {
  message?: string
}

export default function ErrorMessage({ message = 'Ocorreu um erro.' }: ErrorMessageProps) {
  return (
    <div className="bg-red-50 border border-red-200 text-red-700 rounded-lg px-4 py-3">
      {message}
    </div>
  )
}
interface StatusViewProps {
  loading: boolean
  error: string | null
  isEmpty: boolean
  loadingText: string
  emptyText: string
  errorText?: string
}

export function StatusView({
  loading,
  error,
  isEmpty,
  loadingText,
  emptyText,
  errorText = 'Nie udało się pobrać danych'
}: StatusViewProps) {
  if (loading) {
    return <div className="state state-loading">{loadingText}</div>
  }

  if (error) {
    return (
      <div className="state state-error">
        <strong>{errorText}</strong>
        <span>{error}</span>
      </div>
    )
  }

  if (isEmpty) {
    return <div className="state state-empty">{emptyText}</div>
  }

  return null
}

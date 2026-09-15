export function formatCurrency(value?: number | string): string {
  const numericValue = Number(value ?? 0)

  return new Intl.NumberFormat('pl-PL', {
    style: 'currency',
    currency: 'PLN'
  }).format(Number.isFinite(numericValue) ? numericValue : 0)
}

export function formatDate(value?: string): string {
  if (!value || value.startsWith('0001-')) {
    return '-'
  }

  const date = new Date(value)

  if (Number.isNaN(date.getTime())) {
    return value
  }

  return new Intl.DateTimeFormat('pl-PL', {
    dateStyle: 'medium'
  }).format(date)
}

export function formatDateTime(value?: string): string {
  if (!value || value.startsWith('0001-')) {
    return '-'
  }

  const date = new Date(value)

  if (Number.isNaN(date.getTime())) {
    return value
  }

  return new Intl.DateTimeFormat('pl-PL', {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(date)
}

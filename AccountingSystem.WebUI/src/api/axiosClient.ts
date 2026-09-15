import axios, { AxiosError } from 'axios'

export const API_BASE_URL = (
  import.meta.env.VITE_API_BASE_URL || 'https://localhost:7002'
).replace(/\/+$/, '')

export const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    return getAxiosErrorMessage(error)
  }

  if (error instanceof Error) {
    return error.message
  }

  return 'Nieznany błąd'
}

function getAxiosErrorMessage(error: AxiosError): string {
  const responseData = error.response?.data

  if (!responseData) {
    return error.message || 'Nie udało się połączyć z API'
  }

  if (Array.isArray(responseData)) {
    return responseData
      .map((item) => (typeof item === 'string' ? item : JSON.stringify(item)))
      .join(' ')
  }

  if (typeof responseData === 'object') {
    return JSON.stringify(responseData)
  }

  return String(responseData)
}

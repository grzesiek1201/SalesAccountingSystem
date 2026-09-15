import { useEffect, useState } from 'react'
import { axiosClient } from '../../api/axiosClient'
import { Button } from '../Button/Button'

type ApiStatus = 'unknown' | 'checking' | 'ok' | 'error'

export function ApiStatusBadge() {
  const [status, setStatus] = useState<ApiStatus>('unknown')
  const [label, setLabel] = useState('API')

  async function checkApi() {
    setStatus('checking')
    setLabel('Sprawdzam API')

    try {
      const controller = new AbortController()
      const timeoutId = window.setTimeout(() => controller.abort(), 5000)
      const response = await axiosClient.get('/api/customers', {
        signal: controller.signal
      })

      window.clearTimeout(timeoutId)
      setStatus('ok')
      setLabel(Array.isArray(response.data) ? `API OK (${response.data.length})` : 'API OK')
    } catch {
      setStatus('error')
      setLabel('API niedostępne')
    }
  }

  useEffect(() => {
    void checkApi()
  }, [])

  return (
    <div className="api-status">
      <Button variant="ghost" size="sm" onClick={checkApi}>
        Test API
      </Button>
      <span className={`api-dot api-${status}`} />
      <span>{label}</span>
    </div>
  )
}

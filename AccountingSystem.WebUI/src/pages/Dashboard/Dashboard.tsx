import { useEffect, useMemo, useState } from 'react'
import { getApiErrorMessage } from '../../api/axiosClient'
import { DataTable } from '../../components/Table/DataTable'
import { StatusView } from '../../components/State/StatusView'
import { customerService } from '../../services/customerService'
import { productService } from '../../services/productService'
import { quotationService } from '../../services/quotationService'
import { orderService } from '../../services/orderService'
import { invoiceService } from '../../services/invoiceService'
import { formatDateTime } from '../../utils/formatters'
import type { Customer } from '../../models/Customer'
import type { Product } from '../../models/Product'
import type {
  InvoiceDocument,
  Order,
  Quotation,
  SalesDocument
} from '../../models/SalesDocument'

interface DashboardData {
  customers: Customer[]
  products: Product[]
  quotations: Quotation[]
  orders: Order[]
  invoices: InvoiceDocument[]
}

const emptyDashboard: DashboardData = {
  customers: [],
  products: [],
  quotations: [],
  orders: [],
  invoices: []
}

export function Dashboard() {
  const [data, setData] = useState<DashboardData>(emptyDashboard)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  async function loadDashboard() {
    setLoading(true)
    setError(null)

    try {
      const [customers, products, quotations, orders, invoices] =
        await Promise.all([
          customerService.getAll(),
          productService.getAll(),
          quotationService.getAll(),
          orderService.getAll(),
          invoiceService.getAll()
        ])

      setData({ customers, products, quotations, orders, invoices })
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void loadDashboard()
  }, [])

  const recentDocuments = useMemo(() => {
    const documents: Array<SalesDocument & { kind: string; number: string }> = [
      ...data.quotations.map((item) => ({
        ...item,
        kind: 'Oferta',
        number: item.quotationNumber
      })),
      ...data.orders.map((item) => ({
        ...item,
        kind: 'Zamówienie',
        number: item.orderNumber
      })),
      ...data.invoices.map((item) => ({
        ...item,
        kind: 'Faktura',
        number: item.invoiceNumber
      }))
    ]

    return documents
      .sort(
        (left, right) =>
          new Date(right.dateCreated).getTime() -
          new Date(left.dateCreated).getTime()
      )
      .slice(0, 8)
  }, [data])

  const totalDocuments =
    data.quotations.length + data.orders.length + data.invoices.length

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <span className="eyebrow">Przegląd</span>
          <h2>Dashboard</h2>
        </div>
      </div>

      <StatusView
        loading={loading}
        error={error}
        isEmpty={false}
        loadingText="Ładowanie dashboardu..."
        emptyText="Brak danych"
      />

      {!loading && !error && (
        <>
          <div className="stats-grid">
            <SummaryTile label="Klienci" value={data.customers.length} />
            <SummaryTile label="Produkty" value={data.products.length} />
            <SummaryTile label="Dokumenty sprzedaży" value={totalDocuments} />
            <SummaryTile label="Faktury" value={data.invoices.length} />
          </div>

          <section className="panel">
            <div className="section-heading">
              <h3>Ostatnie dokumenty</h3>
            </div>

            <DataTable
              data={recentDocuments}
              getRowKey={(item) => `${item.kind}-${item.id}`}
              emptyText="Brak dokumentów"
              columns={[
                { header: 'Typ', render: (item) => item.kind },
                { header: 'Numer', render: (item) => item.number || '-' },
                {
                  header: 'Klient',
                  render: (item) => item.customer?.name || '-'
                },
                { header: 'Status', render: (item) => item.status || '-' },
                {
                  header: 'Utworzono',
                  render: (item) => formatDateTime(item.dateCreated)
                }
              ]}
            />
          </section>
        </>
      )}
    </section>
  )
}

function SummaryTile({ label, value }: { label: string; value: number }) {
  return (
    <article className="summary-tile">
      <span>{label}</span>
      <strong>{value}</strong>
    </article>
  )
}

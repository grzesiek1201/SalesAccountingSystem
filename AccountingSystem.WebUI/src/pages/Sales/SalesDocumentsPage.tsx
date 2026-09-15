import { FormEvent, useCallback, useEffect, useMemo, useState } from 'react'
import { NavLink } from 'react-router-dom'
import { getApiErrorMessage } from '../../api/axiosClient'
import { StatusBadge } from '../../components/Badge/StatusBadge'
import { Button } from '../../components/Button/Button'
import { FormField } from '../../components/Form/FormField'
import { Modal } from '../../components/Modal/Modal'
import { StatusView } from '../../components/State/StatusView'
import { DataTable } from '../../components/Table/DataTable'
import { customerService } from '../../services/customerService'
import { invoiceService } from '../../services/invoiceService'
import { orderService } from '../../services/orderService'
import { paymentService } from '../../services/paymentService'
import { productService } from '../../services/productService'
import { quotationService } from '../../services/quotationService'
import { formatCurrency, formatDate, formatDateTime } from '../../utils/formatters'
import type { Customer } from '../../models/Customer'
import type { Payment } from '../../models/Payment'
import type { Product } from '../../models/Product'
import type {
  DocumentLinePayload,
  InvoiceDocument,
  SalesDocument,
  SalesDocumentType
} from '../../models/SalesDocument'

interface SalesDocumentsPageProps {
  type: SalesDocumentType
}

interface DocumentFormItem {
  productId: number
  quantity: number
  discountPercent: number
}

interface SalesLabels {
  title: string
  eyebrow: string
  listTitle: string
  numberLabel: string
  createLabel: string
  loadingText: string
  emptyText: string
}

interface DocumentAction {
  label: string
  tone?: 'primary' | 'secondary' | 'danger' | 'success'
  run: (id: number) => Promise<void>
}

const salesLabels: Record<SalesDocumentType, SalesLabels> = {
  quotation: {
    title: 'Oferty',
    eyebrow: 'Sprzedaż / Quotations',
    listTitle: 'Lista ofert',
    numberLabel: 'Numer oferty',
    createLabel: 'Utwórz ofertę',
    loadingText: 'Ładowanie ofert...',
    emptyText: 'Brak ofert'
  },
  order: {
    title: 'Zamówienia',
    eyebrow: 'Sprzedaż / Orders',
    listTitle: 'Lista zamówień',
    numberLabel: 'Numer zamówienia',
    createLabel: 'Utwórz zamówienie',
    loadingText: 'Ładowanie zamówień...',
    emptyText: 'Brak zamówień'
  },
  invoice: {
    title: 'Faktury',
    eyebrow: 'Sprzedaż / Invoices',
    listTitle: 'Lista faktur',
    numberLabel: 'Numer faktury',
    createLabel: 'Utwórz fakturę',
    loadingText: 'Ładowanie faktur...',
    emptyText: 'Brak faktur'
  }
}

export function SalesDocumentsPage({ type }: SalesDocumentsPageProps) {
  const labels = salesLabels[type]
  const [documents, setDocuments] = useState<SalesDocument[]>([])
  const [customers, setCustomers] = useState<Customer[]>([])
  const [products, setProducts] = useState<Product[]>([])
  const [selectedDocument, setSelectedDocument] = useState<SalesDocument | null>(
    null
  )
  const [customerId, setCustomerId] = useState('')
  const [documentNumber, setDocumentNumber] = useState('')
  const [items, setItems] = useState<DocumentFormItem[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)

  const loadSalesData = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const [nextDocuments, nextCustomers, nextProducts] = await Promise.all([
        getDocuments(type),
        customerService.getAll(),
        productService.getAll()
      ])

      setDocuments(nextDocuments)
      setCustomers(nextCustomers)
      setProducts(nextProducts)
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }, [type])

  useEffect(() => {
    setSelectedDocument(null)
    setCustomerId('')
    setDocumentNumber('')
    setItems([])
    void loadSalesData()
  }, [loadSalesData, type])

  const filteredDocuments = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase()

    return documents
      .filter((document) => {
        if (!normalizedSearch) {
          return true
        }

        return [
          getDocumentNumber(type, document),
          document.customer?.name,
          document.status
        ]
          .join(' ')
          .toLowerCase()
          .includes(normalizedSearch)
      })
      .sort(
        (left, right) =>
          new Date(right.dateCreated).getTime() -
          new Date(left.dateCreated).getTime()
      )
  }, [documents, search, type])

  function addItem() {
    setItems((current) => [
      ...current,
      {
        productId: products[0]?.id || 0,
        quantity: 1,
        discountPercent: 0
      }
    ])
  }

  function updateItem(
    index: number,
    field: keyof DocumentFormItem,
    value: number
  ) {
    setItems((current) =>
      current.map((item, itemIndex) =>
        itemIndex === index ? { ...item, [field]: value } : item
      )
    )
  }

  function removeItem(index: number) {
    setItems((current) => current.filter((_, itemIndex) => itemIndex !== index))
  }

  async function createDocument(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      await createDocumentByType(
        type,
        Number(customerId),
        documentNumber.trim(),
        items.map((item, index) => ({
          productId: Number(item.productId),
          quantity: Number(item.quantity),
          discountPercent: Number(item.discountPercent),
          position: index
        }))
      )

      setCustomerId('')
      setDocumentNumber('')
      setItems([])
      await loadSalesData()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  async function refreshSelectedDocument(documentId: number) {
    const refreshed = await getDocument(type, documentId)
    setSelectedDocument(refreshed)
    await loadSalesData()
  }

  async function runAction(action: DocumentAction, documentId: number) {
    setActionError(null)

    try {
      await action.run(documentId)
      await refreshSelectedDocument(documentId)
    } catch (err) {
      setActionError(getApiErrorMessage(err))
    }
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <span className="eyebrow">{labels.eyebrow}</span>
          <h2>{labels.title}</h2>
        </div>
        <Button variant="secondary" onClick={loadSalesData} disabled={loading}>
          Odśwież
        </Button>
      </div>

      <nav className="tabs" aria-label="Sprzedaż">
        <NavLink to="/sales/quotations" className={getTabClassName}>
          Oferty
        </NavLink>
        <NavLink to="/sales/orders" className={getTabClassName}>
          Zamówienia
        </NavLink>
        <NavLink to="/sales/invoices" className={getTabClassName}>
          Faktury
        </NavLink>
      </nav>

      <div className="toolbar">
        <input
          className="input"
          placeholder="Szukaj po numerze, kliencie lub statusie"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
      </div>

      <div className="split-view">
        <section className="panel">
          <div className="section-heading">
            <h3>{labels.listTitle}</h3>
          </div>

          <StatusView
            loading={loading}
            error={error}
            isEmpty={documents.length === 0}
            loadingText={labels.loadingText}
            emptyText={labels.emptyText}
          />

          {!loading && !error && documents.length > 0 && (
            <DataTable
              data={filteredDocuments}
              getRowKey={(document) => document.id}
              emptyText="Brak dokumentów dla podanego filtra"
              columns={[
                {
                  header: 'Numer',
                  render: (document) => (
                    <button
                      className="link-button"
                      onClick={() => setSelectedDocument(document)}
                    >
                      {getDocumentNumber(type, document) || '-'}
                    </button>
                  )
                },
                {
                  header: 'Klient',
                  render: (document) => document.customer?.name || '-'
                },
                {
                  header: 'Data',
                  render: (document) => formatDateTime(document.dateCreated)
                },
                {
                  header: 'Status',
                  render: (document) => (
                    <StatusBadge tone={getStatusTone(document.status)}>
                      {document.status || '-'}
                    </StatusBadge>
                  )
                },
                {
                  header: 'Pozycje',
                  render: (document) => document.items?.length ?? 0
                },
                {
                  header: 'Akcje',
                  render: (document) => (
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => setSelectedDocument(document)}
                    >
                      Szczegóły
                    </Button>
                  )
                }
              ]}
            />
          )}
        </section>

        <aside className="panel side-panel">
          <div className="section-heading">
            <h3>{labels.createLabel}</h3>
          </div>

          <form className="form-grid" onSubmit={createDocument}>
            <FormField label="Klient">
              <select
                className="input"
                value={customerId}
                onChange={(event) => setCustomerId(event.target.value)}
                required
              >
                <option value="">Wybierz klienta</option>
                {customers.map((customer) => (
                  <option key={customer.id} value={customer.id}>
                    {customer.name}
                  </option>
                ))}
              </select>
            </FormField>

            <FormField label={labels.numberLabel}>
              <input
                className="input"
                value={documentNumber}
                onChange={(event) => setDocumentNumber(event.target.value)}
              />
            </FormField>

            <div className="line-items">
              <div className="section-heading compact">
                <h4>Pozycje</h4>
                <Button type="button" variant="secondary" size="sm" onClick={addItem}>
                  Dodaj pozycję
                </Button>
              </div>

              {items.length === 0 && (
                <div className="state state-empty">Brak pozycji</div>
              )}

              {items.map((item, index) => (
                <div className="line-item" key={`${item.productId}-${index}`}>
                  <select
                    className="input"
                    value={item.productId}
                    onChange={(event) =>
                      updateItem(index, 'productId', Number(event.target.value))
                    }
                    required
                  >
                    {products.map((product) => (
                      <option key={product.id} value={product.id}>
                        {product.name}
                      </option>
                    ))}
                  </select>
                  <input
                    className="input"
                    type="number"
                    min="0.01"
                    step="0.01"
                    value={item.quantity}
                    onChange={(event) =>
                      updateItem(index, 'quantity', Number(event.target.value))
                    }
                  />
                  <input
                    className="input"
                    type="number"
                    min="0"
                    step="1"
                    value={item.discountPercent}
                    onChange={(event) =>
                      updateItem(
                        index,
                        'discountPercent',
                        Number(event.target.value)
                      )
                    }
                  />
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    onClick={() => removeItem(index)}
                  >
                    Usuń
                  </Button>
                </div>
              ))}
            </div>

            <Button
              type="submit"
              disabled={
                saving ||
                !customerId ||
                items.length === 0 ||
                products.length === 0
              }
            >
              {saving ? 'Zapisywanie...' : labels.createLabel}
            </Button>
          </form>
        </aside>
      </div>

      {selectedDocument && (
        <DocumentDetailsModal
          type={type}
          document={selectedDocument}
          actionError={actionError}
          onClose={() => {
            setSelectedDocument(null)
            setActionError(null)
          }}
          onAction={runAction}
        />
      )}
    </section>
  )
}

function DocumentDetailsModal({
  type,
  document,
  actionError,
  onClose,
  onAction
}: {
  type: SalesDocumentType
  document: SalesDocument
  actionError: string | null
  onClose: () => void
  onAction: (action: DocumentAction, documentId: number) => Promise<void>
}) {
  const actions = getActions(type)
  const number = getDocumentNumber(type, document)
  const conversionAction = getConversionAction(type)

  return (
    <Modal
      title={`Szczegóły: ${number || `#${document.id}`}`}
      onClose={onClose}
      footer={
        <div className="button-row">
          {conversionAction && (
            <Button
              variant="success"
              onClick={() => onAction(conversionAction, document.id)}
            >
              {conversionAction.label}
            </Button>
          )}
          {actions.map((action) => (
            <Button
              key={action.label}
              variant={action.tone || 'secondary'}
              onClick={() => onAction(action, document.id)}
            >
              {action.label}
            </Button>
          ))}
        </div>
      }
    >
      {actionError && (
        <div className="state state-error">
          <strong>Nie udało się wykonać operacji</strong>
          <span>{actionError}</span>
        </div>
      )}

      <div className="details-grid">
        <Detail label="Klient" value={document.customer?.name || '-'} />
        <Detail label="Status" value={document.status || '-'} />
        <Detail label="Utworzono" value={formatDateTime(document.dateCreated)} />
        {type === 'invoice' && isInvoiceDocument(document) && (
          <>
            <Detail label="Data wystawienia" value={formatDate(document.issueDate)} />
            <Detail label="Termin płatności" value={formatDate(document.dueDate)} />
          </>
        )}
      </div>

      <section className="details-section">
        <h3>Pozycje</h3>
        <DataTable
          data={document.items || []}
          getRowKey={(item, indexFallback) =>
            item.id || `${item.productName}-${indexFallback}`
          }
          emptyText="Brak pozycji"
          columns={[
            {
              header: '#',
              render: (item) => item.position ?? '-'
            },
            {
              header: 'Produkt',
              render: (item) => item.productName || '-'
            },
            {
              header: 'Kod',
              render: (item) => item.productCode || '-'
            },
            {
              header: 'Ilość',
              render: (item) => item.quantity
            },
            {
              header: 'Cena',
              render: (item) => formatCurrency(item.baseUnitPrice)
            },
            {
              header: 'Razem',
              render: (item) => formatCurrency(item.total)
            }
          ]}
        />
      </section>

      {type === 'invoice' && <PaymentsPanel invoiceId={document.id} />}
    </Modal>
  )
}

function PaymentsPanel({ invoiceId }: { invoiceId: number }) {
  const [payments, setPayments] = useState<Payment[]>([])
  const [amount, setAmount] = useState('')
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadPayments = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      setPayments(await paymentService.getForInvoice(invoiceId))
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }, [invoiceId])

  useEffect(() => {
    void loadPayments()
  }, [loadPayments])

  async function createPayment(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      await paymentService.create({
        invoiceId,
        amount: Number(amount)
      })
      setAmount('')
      await loadPayments()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  async function deletePayment(paymentId: number) {
    if (!window.confirm('Usunąć płatność?')) {
      return
    }

    setSaving(true)
    setError(null)

    try {
      await paymentService.delete(paymentId)
      await loadPayments()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  return (
    <section className="details-section">
      <h3>Płatności</h3>

      <form className="inline-form" onSubmit={createPayment}>
        <input
          className="input"
          type="number"
          min="0.01"
          step="0.01"
          placeholder="Kwota"
          value={amount}
          onChange={(event) => setAmount(event.target.value)}
        />
        <Button type="submit" disabled={saving || !amount}>
          Dodaj płatność
        </Button>
      </form>

      <StatusView
        loading={loading}
        error={error}
        isEmpty={payments.length === 0}
        loadingText="Ładowanie płatności..."
        emptyText="Brak płatności"
      />

      {!loading && !error && payments.length > 0 && (
        <DataTable
          data={payments}
          getRowKey={(payment) => payment.id}
          columns={[
            { header: 'ID', render: (payment) => payment.id },
            {
              header: 'Kwota',
              render: (payment) => formatCurrency(payment.amount)
            },
            {
              header: 'Data',
              render: (payment) => formatDateTime(payment.paymentDate)
            },
            { header: 'Status', render: (payment) => payment.status },
            {
              header: 'Akcje',
              render: (payment) => (
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => deletePayment(payment.id)}
                >
                  Usuń
                </Button>
              )
            }
          ]}
        />
      )}
    </section>
  )
}

function Detail({ label, value }: { label: string; value: string }) {
  return (
    <div className="detail-item">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  )
}

async function getDocuments(type: SalesDocumentType): Promise<SalesDocument[]> {
  if (type === 'quotation') {
    return quotationService.getAll()
  }

  if (type === 'order') {
    return orderService.getAll()
  }

  return invoiceService.getAll()
}

async function getDocument(
  type: SalesDocumentType,
  id: number
): Promise<SalesDocument> {
  if (type === 'quotation') {
    return quotationService.getById(id)
  }

  if (type === 'order') {
    return orderService.getById(id)
  }

  return invoiceService.getById(id)
}

async function createDocumentByType(
  type: SalesDocumentType,
  customerId: number,
  documentNumber: string,
  items: DocumentLinePayload[]
) {
  if (type === 'quotation') {
    await quotationService.create({
      customerId,
      quotationNumber: documentNumber || undefined,
      items
    })
    return
  }

  if (type === 'order') {
    await orderService.create({
      customerId,
      orderNumber: documentNumber || undefined,
      items
    })
    return
  }

  await invoiceService.create({
    customerId,
    invoiceNumber: documentNumber || undefined,
    items
  })
}

function getDocumentNumber(
  type: SalesDocumentType,
  document: SalesDocument
): string {
  if (type === 'quotation' && 'quotationNumber' in document) {
    return document.quotationNumber
  }

  if (type === 'order' && 'orderNumber' in document) {
    return document.orderNumber
  }

  if (type === 'invoice' && 'invoiceNumber' in document) {
    return document.invoiceNumber
  }

  return ''
}

function getActions(type: SalesDocumentType): DocumentAction[] {
  if (type === 'quotation') {
    return [
      { label: 'Wyślij', run: quotationService.send },
      { label: 'Akceptuj', run: quotationService.accept, tone: 'success' },
      { label: 'Odrzuć', run: quotationService.reject, tone: 'danger' }
    ]
  }

  if (type === 'order') {
    return [
      { label: 'Potwierdź', run: orderService.confirm },
      { label: 'Zakończ', run: orderService.complete, tone: 'success' },
      { label: 'Anuluj', run: orderService.cancel, tone: 'danger' }
    ]
  }

  return [
    { label: 'Wystaw', run: invoiceService.issue, tone: 'success' },
    { label: 'Anuluj', run: invoiceService.cancel, tone: 'danger' },
    { label: 'Archiwizuj', run: invoiceService.archive }
  ]
}

function getConversionAction(type: SalesDocumentType): DocumentAction | null {
  if (type === 'quotation') {
    return {
      label: 'Konwertuj na zamówienie',
      tone: 'success',
      run: quotationService.convertToOrder
    }
  }

  if (type === 'order') {
    return {
      label: 'Konwertuj na fakturę',
      tone: 'success',
      run: orderService.convertToInvoice
    }
  }

  return null
}

function getTabClassName({ isActive }: { isActive: boolean }) {
  return isActive ? 'tab active' : 'tab'
}

function getStatusTone(status: string): 'neutral' | 'success' | 'warning' | 'danger' {
  const normalizedStatus = status.toLowerCase()

  if (
    normalizedStatus.includes('accepted') ||
    normalizedStatus.includes('confirmed') ||
    normalizedStatus.includes('completed') ||
    normalizedStatus.includes('issued') ||
    normalizedStatus.includes('paid')
  ) {
    return 'success'
  }

  if (
    normalizedStatus.includes('reject') ||
    normalizedStatus.includes('cancel') ||
    normalizedStatus.includes('overdue')
  ) {
    return 'danger'
  }

  if (normalizedStatus.includes('draft') || normalizedStatus.includes('sent')) {
    return 'warning'
  }

  return 'neutral'
}

function isInvoiceDocument(document: SalesDocument): document is InvoiceDocument {
  return 'invoiceNumber' in document
}

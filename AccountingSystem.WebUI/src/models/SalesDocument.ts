import type { Customer } from './Customer'

export type SalesDocumentType = 'quotation' | 'order' | 'invoice'

export interface SalesDocumentItem {
  id?: number
  productId?: number
  productName?: string
  productCode?: string
  quantity: number
  discountPercent?: number
  baseUnitPrice?: number
  total?: number
  position?: number
}

export interface DocumentLinePayload {
  productId: number
  quantity: number
  discountPercent: number
  position: number
}

export interface BaseSalesDocument {
  id: number
  status: string
  dateCreated: string
  customer: Customer
  items: SalesDocumentItem[]
}

export interface Quotation extends BaseSalesDocument {
  quotationNumber: string
}

export interface Order extends BaseSalesDocument {
  orderNumber: string
}

export interface InvoiceDocument extends BaseSalesDocument {
  invoiceNumber: string
  issueDate: string
  dueDate: string
}

export type SalesDocument = Quotation | Order | InvoiceDocument

export interface CreateQuotationPayload {
  customerId: number
  quotationNumber?: string
  items: DocumentLinePayload[]
}

export interface CreateOrderPayload {
  customerId: number
  orderNumber?: string
  items: DocumentLinePayload[]
}

export interface CreateInvoicePayload {
  customerId: number
  invoiceNumber?: string
  items: DocumentLinePayload[]
}

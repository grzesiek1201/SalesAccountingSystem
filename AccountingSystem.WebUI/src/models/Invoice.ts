import type { Customer } from './Customer'
import type { SalesDocumentItem } from './SalesDocument'

export interface Invoice {
  id: number
  invoiceNumber: string
  status: string
  dateCreated: string
  issueDate: string
  dueDate: string
  customer: Customer
  items: SalesDocumentItem[]
}

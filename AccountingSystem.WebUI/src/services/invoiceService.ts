import { axiosClient } from '../api/axiosClient'
import type {
  CreateInvoicePayload,
  InvoiceDocument
} from '../models/SalesDocument'

const endpoint = '/api/invoices'

export const invoiceService = {
  async getAll(): Promise<InvoiceDocument[]> {
    const response = await axiosClient.get<InvoiceDocument[]>(endpoint)
    return response.data ?? []
  },

  async getById(id: number): Promise<InvoiceDocument> {
    const response = await axiosClient.get<InvoiceDocument>(`${endpoint}/${id}`)
    return response.data
  },

  async create(payload: CreateInvoicePayload): Promise<void> {
    await axiosClient.post(endpoint, payload)
  },

  async archive(id: number): Promise<void> {
    await axiosClient.patch(`${endpoint}/${id}/archive`)
  },

  async issue(id: number): Promise<void> {
    await axiosClient.post(`${endpoint}/${id}/issue`)
  },

  async cancel(id: number): Promise<void> {
    await axiosClient.post(`${endpoint}/${id}/cancel`)
  }
}

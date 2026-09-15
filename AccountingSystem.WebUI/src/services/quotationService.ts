import { axiosClient } from '../api/axiosClient'
import type {
  CreateQuotationPayload,
  Quotation
} from '../models/SalesDocument'

const endpoint = '/api/quotations'

export const quotationService = {
  async getAll(): Promise<Quotation[]> {
    const response = await axiosClient.get<Quotation[]>(endpoint)
    return response.data ?? []
  },

  async getById(id: number): Promise<Quotation> {
    const response = await axiosClient.get<Quotation>(`${endpoint}/${id}`)
    return response.data
  },

  async create(payload: CreateQuotationPayload): Promise<void> {
    await axiosClient.post(endpoint, payload)
  },

  async archive(id: number): Promise<void> {
    await axiosClient.patch(`${endpoint}/${id}/archive`)
  },

  async send(id: number): Promise<void> {
    await axiosClient.post(`${endpoint}/${id}/send`)
  },

  async accept(id: number): Promise<void> {
    await axiosClient.post(`${endpoint}/${id}/accept`)
  },

  async reject(id: number): Promise<void> {
    await axiosClient.post(`${endpoint}/${id}/reject`)
  },

  async convertToOrder(id: number): Promise<void> {
    await axiosClient.post(`/api/orders/from-quotation/${id}`)
  }
}

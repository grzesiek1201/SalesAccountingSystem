import { axiosClient } from '../api/axiosClient'
import type { CreateOrderPayload, Order } from '../models/SalesDocument'

const endpoint = '/api/orders'

export const orderService = {
  async getAll(): Promise<Order[]> {
    const response = await axiosClient.get<Order[]>(endpoint)
    return response.data ?? []
  },

  async getById(id: number): Promise<Order> {
    const response = await axiosClient.get<Order>(`${endpoint}/${id}`)
    return response.data
  },

  async create(payload: CreateOrderPayload): Promise<void> {
    await axiosClient.post(endpoint, payload)
  },

  async archive(id: number): Promise<void> {
    await axiosClient.patch(`${endpoint}/${id}/archive`)
  },

  async confirm(id: number): Promise<void> {
    await axiosClient.post(`${endpoint}/${id}/confirm`)
  },

  async complete(id: number): Promise<void> {
    await axiosClient.post(`${endpoint}/${id}/complete`)
  },

  async cancel(id: number): Promise<void> {
    await axiosClient.post(`${endpoint}/${id}/cancel`)
  },

  async convertToInvoice(id: number): Promise<void> {
    await axiosClient.post(`/api/invoices/from-order/${id}`)
  }
}

import { axiosClient } from '../api/axiosClient'
import type { Customer, CustomerPayload } from '../models/Customer'

const endpoint = '/api/customers'

export const customerService = {
  async getAll(): Promise<Customer[]> {
    const response = await axiosClient.get<Customer[]>(endpoint)
    return response.data ?? []
  },

  async getById(id: number): Promise<Customer> {
    const response = await axiosClient.get<Customer>(`${endpoint}/${id}`)
    return response.data
  },

  async create(payload: CustomerPayload): Promise<void> {
    await axiosClient.post(endpoint, payload)
  },

  async update(id: number, payload: CustomerPayload): Promise<Customer> {
    const response = await axiosClient.put<Customer>(`${endpoint}/${id}`, {
      ...payload,
      id
    })

    return response.data
  }
}

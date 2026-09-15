import { axiosClient } from '../api/axiosClient'
import type { Product, ProductPayload } from '../models/Product'

const endpoint = '/api/products'

export const productService = {
  async getAll(): Promise<Product[]> {
    const response = await axiosClient.get<Product[]>(endpoint)
    return response.data ?? []
  },

  async getById(id: number): Promise<Product> {
    const response = await axiosClient.get<Product>(`${endpoint}/${id}`)
    return response.data
  },

  async create(payload: ProductPayload): Promise<void> {
    await axiosClient.post(endpoint, payload)
  },

  async update(id: number, payload: ProductPayload): Promise<Product> {
    const response = await axiosClient.put<Product>(`${endpoint}/${id}`, {
      ...payload,
      id
    })

    return response.data
  }
}

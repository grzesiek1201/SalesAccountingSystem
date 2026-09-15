import { axiosClient } from '../api/axiosClient'
import type {
  ProductCategory,
  ProductCategoryPayload
} from '../models/Product'

const endpoint = '/api/product-categories'

export const productCategoryService = {
  async getAll(): Promise<ProductCategory[]> {
    const response = await axiosClient.get<ProductCategory[]>(endpoint)
    return response.data ?? []
  },

  async create(payload: ProductCategoryPayload): Promise<void> {
    await axiosClient.post(endpoint, payload)
  },

  async update(id: number, payload: ProductCategoryPayload): Promise<void> {
    await axiosClient.put(`${endpoint}/${id}`, {
      ...payload,
      id
    })
  }
}

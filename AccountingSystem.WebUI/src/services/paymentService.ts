import { axiosClient } from '../api/axiosClient'
import type { Payment, PaymentPayload } from '../models/Payment'

const endpoint = '/api/payments'

export const paymentService = {
  async getForInvoice(invoiceId: number): Promise<Payment[]> {
    const response = await axiosClient.get<Payment[]>(
      `${endpoint}/invoice/${invoiceId}`
    )
    return response.data ?? []
  },

  async create(payload: PaymentPayload): Promise<void> {
    await axiosClient.post(endpoint, payload)
  },

  async delete(paymentId: number): Promise<void> {
    await axiosClient.delete(`${endpoint}/${paymentId}`)
  }
}

export interface Payment {
  id: number
  invoiceId: number
  amount: number
  paymentDate: string
  status: string
}

export interface PaymentPayload {
  invoiceId: number
  amount: number
}

export interface Customer {
  id: number
  name: string
  nip: string
  email: string
  street: string
  city: string
  zipCode: string
}

export type CustomerPayload = Omit<Customer, 'id'>

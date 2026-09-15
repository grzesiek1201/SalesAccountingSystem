export type ProductUnit =
  | 'Piece'
  | 'Kilogram'
  | 'Liter'
  | 'Meter'
  | 'Hour'
  | 'Package'

export interface ProductCategory {
  id: number
  name: string
  isActive: boolean
}

export interface Product {
  id: number
  name: string
  productCode: string
  vatRate: number
  unit: ProductUnit
  price: number
  categoryId: number
}

export interface ProductPayload {
  name: string
  productCode: string
  vatRate: number
  unit: ProductUnit
  price: number
  categoryId: number
}

export interface ProductCategoryPayload {
  name: string
}

export interface Category {
  id: string
  name: string
}

export interface Product {
  id: string
  name: string
  description: string | null
  price: number
  categoryId: string
  categoryName: string | null
  stockQuantity: number
  isLowStock: boolean
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface CategoryProductCount {
  categoryId: string
  categoryName: string
  productCount: number
}

export interface DashboardSummary {
  totalProducts: number
  totalStockValue: number
  lowStockCount: number
  lowStockProducts: Product[]
  productsByCategory: CategoryProductCount[]
}

// Common types
export interface ApiResponse<T> {
  data: T | null
  error: string | null
  success: boolean
}

export interface PaginatedResponse<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

export interface DropdownOption<T = string> {
  value: T
  label: string
  disabled?: boolean
}

// Export all types
export * from './auth'
export * from './persona'
export * from './empleado'
export * from './departamento'
export * from './nomina'
export * from './novedad'
export * from './parametro'
export * from './banking'
export * from './provision'
export * from './workspace'

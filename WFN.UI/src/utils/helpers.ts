// Storage keys
export const STORAGE_KEYS = {
  ACCESS_TOKEN: 'access_token',
  REFRESH_TOKEN: 'refresh_token',
  USER_PROFILE: 'user_profile',
} as const

// Date constants
export const MILLISECONDS_PER_DAY = 1000 * 60 * 60 * 24
export const DAYS_PER_YEAR = 365

/**
 * Formats a Date object to YYYY-MM-DD string for input fields
 */
export function formatDateForInput(date: Date): string {
  return date.toISOString().split('T')[0] ?? ''
}

/**
 * Formats a Date object to YYYY-MM string for month input fields
 */
export function formatMonthForInput(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  return `${year}-${month}`
}

/**
 * Parses a date string to Date object
 */
export function parseDate(dateString: string): Date {
  return new Date(dateString)
}

/**
 * Calculates the difference in years between two dates
 */
export function getYearsDifference(date1: Date, date2: Date): number {
  const diffTime = Math.abs(date2.getTime() - date1.getTime())
  const diffDays = diffTime / MILLISECONDS_PER_DAY
  return diffDays / DAYS_PER_YEAR
}

/**
 * Checks if an employee has been working for more than one year
 */
export function hasWorkedMoreThanOneYear(fechaIngreso: string): boolean {
  const startDate = parseDate(fechaIngreso)
  const today = new Date()
  return getYearsDifference(startDate, today) >= 1
}

/**
 * Formats a number as currency (USD)
 */
export function formatCurrency(value: number): string {
  return `$${value.toFixed(2)}`
}

/**
 * Formats a date string to display format (DD/MM/YYYY)
 */
export function formatDateDisplay(dateString: string): string {
  if (!dateString) return '-'
  const date = parseDate(dateString)
  return date.toLocaleDateString('es-EC', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  })
}

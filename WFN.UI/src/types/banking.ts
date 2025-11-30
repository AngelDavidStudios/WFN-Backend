// Types for Banking model
export interface Banking {
  pk?: string
  sk?: string
  id_Banking: string
  bankName: string
  accountNumber: string
  accountType: AccountType
  swiftCode: string
  pais: string
  sucursal: string
  dateCreated?: string
}

export type AccountType = 'AHORROS' | 'CORRIENTE'

export interface BankingCreateDTO {
  bankName: string
  accountNumber: string
  accountType: AccountType
  swiftCode?: string
  pais: string
  sucursal?: string
}

export interface BankingUpdateDTO extends BankingCreateDTO {
  id_Banking: string
}

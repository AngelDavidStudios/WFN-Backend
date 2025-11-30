// Types for Parametro model
export interface Parametro {
  pk?: string
  sk?: string
  id_Parametro: string
  nombre: string
  tipo: TipoParametro
  tipoCalculo: string
  descripcion: string
  dateCreated?: string
}

export type TipoParametro = 'INGRESO' | 'EGRESO' | 'PROVISION'

export interface ParametroCreateDTO {
  nombre: string
  tipo: TipoParametro
  tipoCalculo: string
  descripcion?: string
}

export interface ParametroUpdateDTO extends ParametroCreateDTO {
  id_Parametro: string
}

// Tipos de cálculo disponibles
export type TipoCalculoIngreso =
  | 'HORAS_EXTRAS_50'
  | 'HORAS_EXTRAS_100'
  | 'SIMPLE'
  | 'DECIMO_TERCERO'
  | 'DECIMO_CUARTO'
  | 'FONDOS_RESERVA'

export type TipoCalculoEgreso = 'IESS_PERSONAL' | 'IESS_EXTENSION' | 'IMPUESTO_RENTA' | 'EGRESO'

export type TipoCalculoProvision =
  | 'PROVISION_VACACIONES'
  | 'IESS_PATRONAL'
  | 'PROVISION_DECIMO_TERCERO'
  | 'PROVISION_DECIMO_CUARTO'
  | 'PROVISION_FONDOS_RESERVA'

// Parametro dropdown option for selects
export interface ParametroDropdownOption {
  value: string
  label: string
  tipo: TipoParametro
  tipoCalculo: string
}

// Types for Novedad model
export interface Novedad {
  pk?: string
  sk?: string
  id_Novedad: string
  id_Parametro: string
  periodo: string
  tipoNovedad: TipoNovedad
  fechaIngresada: string
  descripcion: string
  montoAplicado: number
  is_Gravable: boolean
}

export type TipoNovedad = 'INGRESO' | 'EGRESO' | 'PROVISION'

export interface NovedadCreateDTO {
  id_Parametro: string
  periodo: string
  tipoNovedad: TipoNovedad
  fechaIngresada: string
  descripcion?: string
  montoAplicado: number
  is_Gravable: boolean
}

export interface NovedadUpdateDTO extends NovedadCreateDTO {
  id_Novedad: string
}

// Extended novedad with parameter name for display
export interface NovedadConParametro extends Novedad {
  nombreParametro?: string
  tipoCalculo?: string
}

// Types for Provision model
export interface Provision {
  pk?: string
  sk?: string
  id_Provision: string
  id_Empleado: string
  tipoProvision: TipoProvision
  periodo: string
  valorMensual: number
  acumulado: number
  total: number
  isTransferred: boolean
  fechaCalculo: string
}

export type TipoProvision =
  | 'DECIMO_TERCERO'
  | 'DECIMO_CUARTO'
  | 'FONDO_RESERVA'
  | 'VACACIONES'
  | 'IESS_PATRONAL'

export interface ProvisionCreateDTO {
  tipoProvision: TipoProvision
  periodo: string
  valorMensual: number
  acumulado?: number
  total?: number
  isTransferred?: boolean
}

export interface ProvisionUpdateDTO extends ProvisionCreateDTO {
  id_Provision?: string
}

export interface ProcesarProvisionesRequest {
  empleadoId: string
  periodo: string
}

// Extended provision with employee data for display
export interface ProvisionConEmpleado extends Provision {
  nombreEmpleado?: string
  dniEmpleado?: string
}

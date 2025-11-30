// Enum for employee status
export enum StatusLaboral {
  Active = 0,
  Inactive = 1,
  OnLeave = 2,
  Retired = 3,
}

// Types for Empleado model
export interface Empleado {
  pk?: string
  sk?: string
  id_Empleado: string
  id_Persona: string
  id_Departamento: string
  fechaIngreso: string
  salarioBase: number
  is_DecimoTercMensual: boolean
  is_DecimoCuartoMensual: boolean
  is_FondoReserva: boolean
  dateCreated?: string
  statusLaboral: StatusLaboral
}

export interface EmpleadoCreateDTO {
  id_Persona: string
  id_Departamento: string
  fechaIngreso: string
  salarioBase: number
  is_DecimoTercMensual: boolean
  is_DecimoCuartoMensual: boolean
  is_FondoReserva: boolean
  statusLaboral: StatusLaboral
}

export interface EmpleadoUpdateDTO extends EmpleadoCreateDTO {
  id_Empleado: string
}

// Extended empleado with persona data for display
export interface EmpleadoConPersona extends Empleado {
  nombreCompleto?: string
  dni?: string
  departamentoNombre?: string
}

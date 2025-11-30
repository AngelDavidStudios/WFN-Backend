// Types for Nomina model
export interface Nomina {
  pk?: string
  sk?: string
  id_Nomina: string
  id_Empleado: string
  periodo: string
  totalIngresosGravados: number
  totalIngresosNoGravados: number
  totalIngresos: number
  totalEgresos: number
  iess_AportePersonal: number
  ir_Retenido: number
  netoAPagar: number
  fechaCalculo: string
  isCerrada: boolean
}

export interface GenerarNominaRequest {
  empleadoId: string
  periodo: string
}

export interface RecalcularNominaRequest {
  empleadoId: string
  periodo: string
}

// Extended nomina with employee data for display
export interface NominaConEmpleado extends Nomina {
  nombreEmpleado?: string
  dniEmpleado?: string
  departamentoNombre?: string
}

// Summary for period payroll
export interface ResumenNomina {
  periodo: string
  totalEmpleados: number
  totalIngresosGravados: number
  totalIngresosNoGravados: number
  totalIngresos: number
  totalEgresos: number
  totalNetoAPagar: number
}

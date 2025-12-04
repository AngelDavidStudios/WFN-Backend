// Types for Workspace model
export interface WorkspaceNomina {
  pk?: string
  sk?: string
  id_Workspace: string
  periodo: string
  fechaCreacion: string
  fechaCierre: string
  estado: EstadoWorkspace
}

export enum EstadoWorkspace {
  Abierto = 0,
  Cerrado = 1,
}

export interface WorkspaceEstado {
  periodo: string
  abierto: boolean
  estado: 'ABIERTO' | 'CERRADO'
}

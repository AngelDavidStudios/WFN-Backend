// Types for Departamento model
export interface Departamento {
  pk?: string
  sk?: string
  id_Departamento: string
  nombre: string
  ubicacion: string
  email: string
  telefono: string
  cargo: string
  centroCosto: string
  dateCreated?: string
}

export interface DepartamentoCreateDTO {
  nombre: string
  ubicacion: string
  email: string
  telefono: string
  cargo: string
  centroCosto: string
}

export interface DepartamentoUpdateDTO extends DepartamentoCreateDTO {
  id_Departamento: string
}

// Departamento dropdown option for selects
export interface DepartamentoDropdownOption {
  value: string
  label: string
}

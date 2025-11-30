// Types for Persona model
export interface Direccion {
  calle: string
  numero: string
  piso: string
}

export interface Persona {
  pk?: string
  sk?: string
  id_Persona: string
  dni: string
  gender: string
  primerNombre: string
  segundoNombre: string
  apellidoMaterno: string
  apellidoPaterno: string
  dateBirthday: string
  edad: number
  correo: string[]
  telefono: string[]
  direccion: Direccion
  dateCreated?: string
}

export interface PersonaCreateDTO {
  dni: string
  gender: string
  primerNombre: string
  segundoNombre: string
  apellidoMaterno: string
  apellidoPaterno: string
  dateBirthday: string
  correo: string[]
  telefono: string[]
  direccion: Direccion
}

export interface PersonaUpdateDTO extends PersonaCreateDTO {
  id_Persona: string
}

// Persona dropdown option for selects
export interface PersonaDropdownOption {
  value: string
  label: string
  dni: string
}

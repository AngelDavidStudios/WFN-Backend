import type { DropdownOption } from '@/types'
import { StatusLaboral, EstadoWorkspace } from '@/types'
import type { TipoNovedad, TipoParametro, TipoProvision, AccountType } from '@/types'

// Status laboral options for empleados
export const STATUS_LABORAL_OPTIONS: DropdownOption<StatusLaboral>[] = [
  { value: StatusLaboral.Active, label: 'Activo' },
  { value: StatusLaboral.Inactive, label: 'Inactivo' },
  { value: StatusLaboral.OnLeave, label: 'Con Licencia' },
  { value: StatusLaboral.Retired, label: 'Retirado' },
]

// Estado workspace options
export const ESTADO_WORKSPACE_OPTIONS: DropdownOption<EstadoWorkspace>[] = [
  { value: EstadoWorkspace.Abierto, label: 'Abierto' },
  { value: EstadoWorkspace.Cerrado, label: 'Cerrado' },
]

// Gender options
export const GENDER_OPTIONS: DropdownOption<string>[] = [
  { value: 'M', label: 'Masculino' },
  { value: 'F', label: 'Femenino' },
  { value: 'O', label: 'Otro' },
]

// Tipo novedad options
export const TIPO_NOVEDAD_OPTIONS: DropdownOption<TipoNovedad>[] = [
  { value: 'INGRESO', label: 'Ingreso' },
  { value: 'EGRESO', label: 'Egreso' },
  { value: 'PROVISION', label: 'Provisión' },
]

// Tipo parametro options
export const TIPO_PARAMETRO_OPTIONS: DropdownOption<TipoParametro>[] = [
  { value: 'INGRESO', label: 'Ingreso' },
  { value: 'EGRESO', label: 'Egreso' },
  { value: 'PROVISION', label: 'Provisión' },
]

// Tipos de cálculo para ingresos
// Tipos de cálculo para ingresos
export const TIPOS_CALCULO_INGRESO: DropdownOption<string>[] = [
  { value: 'SALARIO_BASE', label: 'Salario Base' },
  { value: 'VARIABLE', label: 'Variable' },
  { value: 'COMISIONES', label: 'Comisiones' },
  { value: 'TRANSPORTE', label: 'Transporte' },
  { value: 'NRO50', label: 'Horas Extras 50% (NRO50)' },
  { value: 'HORAS_EXTRAS_50', label: 'Horas Extras al 50%' },
  { value: 'NRO100', label: 'Horas Extras 100% (NRO100)' },
  { value: 'HORAS_EXTRAS_100', label: 'Horas Extras al 100%' },
  { value: 'DECIMO_TERCERO_MENSUAL', label: 'Décimo Tercero Mensual' },
  { value: 'DECIMO_CUARTO_MENSUAL', label: 'Décimo Cuarto Mensual' },
  { value: 'FONDOS_RESERVA_MENSUAL', label: 'Fondos de Reserva Mensual' },
]

// Tipos de cálculo para egresos
export const TIPOS_CALCULO_EGRESO: DropdownOption<string>[] = [
  { value: 'IESS_PERSONAL', label: 'IESS Aporte Personal (9.45%)' },
  { value: 'IESS_EXTENSION_CONYUGE', label: 'IESS Extensión Cónyuge (3.41%)' },
  { value: 'IESS_EXTENSION_CONVIVIENTE', label: 'IESS Extensión Conviviente (3.41%)' },
  { value: 'IMPUESTO_RENTA', label: 'Impuesto a la Renta' },
  { value: 'PRESTAMO_QUIROGRAFARIO', label: 'Préstamo Quirografario' },
  { value: 'PRESTAMO_HIPOTECARIO', label: 'Préstamo Hipotecario' },
  { value: 'PRESTAMOS_EMPLEADOS', label: 'Préstamos a Empleados' },
  { value: 'ANTICIPOS_EMPLEADOS', label: 'Anticipos Empleados' },
  { value: 'COMPRA_ALMACEN', label: 'Compra Almacén' },
  { value: 'GIMNASIO', label: 'Gimnasio' },
  { value: 'COMISARIATO', label: 'Comisariato' },
  { value: 'CATERING', label: 'Catering' },
  { value: 'CONSUMO_CELULAR', label: 'Consumo Celular' },
  { value: 'COMPRA_CELULAR', label: 'Compra Celular' },
  { value: 'FALTA_INJUSTIFICADA', label: 'Falta Injustificada' },
  { value: 'SEGURO_VIDA', label: 'Seguro de Vida' },
  { value: 'PENSION_ALIMENTICIA', label: 'Pensión Alimenticia' },
  { value: 'SUBSIDIO_IESS_ENFERMEDAD', label: 'Subsidio IESS Enfermedad' },
  { value: 'NOTA_CREDITO_IESS_MATERNIDAD', label: 'Nota Crédito IESS Maternidad' },
]

// Tipos de cálculo para provisiones
export const TIPOS_CALCULO_PROVISION: DropdownOption<string>[] = [
  { value: 'PROVISION_VACACIONES', label: 'Provisión Vacaciones' },
  { value: 'IESS_PATRONAL', label: 'IESS Patronal (12.15%)' },
  { value: 'PROVISION_DECIMO_TERCERO', label: 'Provisión Décimo Tercero' },
  { value: 'PROVISION_DECIMO_CUARTO', label: 'Provisión Décimo Cuarto' },
  { value: 'PROVISION_FONDOS_RESERVA', label: 'Provisión Fondos de Reserva' },
]

// Tipo provision options
export const TIPO_PROVISION_OPTIONS: DropdownOption<TipoProvision>[] = [
  { value: 'DECIMO_TERCERO', label: 'Décimo Tercero' },
  { value: 'DECIMO_CUARTO', label: 'Décimo Cuarto' },
  { value: 'FONDO_RESERVA', label: 'Fondo de Reserva' },
  { value: 'VACACIONES', label: 'Vacaciones' },
  { value: 'IESS_PATRONAL', label: 'IESS Patronal' },
]

// Account type options for banking
export const ACCOUNT_TYPE_OPTIONS: DropdownOption<AccountType>[] = [
  { value: 'AHORROS', label: 'Cuenta de Ahorros' },
  { value: 'CORRIENTE', label: 'Cuenta Corriente' },
]

// Banks in Ecuador
export const BANCOS_ECUADOR: DropdownOption<string>[] = [
  { value: 'BANCO_PICHINCHA', label: 'Banco Pichincha' },
  { value: 'BANCO_GUAYAQUIL', label: 'Banco de Guayaquil' },
  { value: 'PRODUBANCO', label: 'Produbanco' },
  { value: 'BANCO_PACIFICO', label: 'Banco del Pacífico' },
  { value: 'BANCO_INTERNACIONAL', label: 'Banco Internacional' },
  { value: 'BANCO_BOLIVARIANO', label: 'Banco Bolivariano' },
  { value: 'BANCO_AUSTRO', label: 'Banco del Austro' },
  { value: 'BANCO_LOJA', label: 'Banco de Loja' },
  { value: 'BANCO_MACHALA', label: 'Banco de Machala' },
  { value: 'BANCO_SOLIDARIO', label: 'Banco Solidario' },
  { value: 'COOP_JEP', label: 'Cooperativa JEP' },
  { value: 'COOP_JARDIN_AZUAYO', label: 'Cooperativa Jardín Azuayo' },
  { value: 'OTRO', label: 'Otro' },
]

// Countries (main ones)
export const PAISES_OPTIONS: DropdownOption<string>[] = [
  { value: 'EC', label: 'Ecuador' },
  { value: 'CO', label: 'Colombia' },
  { value: 'PE', label: 'Perú' },
  { value: 'US', label: 'Estados Unidos' },
  { value: 'ES', label: 'España' },
  { value: 'MX', label: 'México' },
  { value: 'AR', label: 'Argentina' },
  { value: 'CL', label: 'Chile' },
  { value: 'BR', label: 'Brasil' },
  { value: 'OTRO', label: 'Otro' },
]

// Helper function to get tipo calculo options based on tipo parametro
export function getTipoCalculoOptions(tipoParametro: TipoParametro): DropdownOption<string>[] {
  switch (tipoParametro) {
    case 'INGRESO':
      return TIPOS_CALCULO_INGRESO
    case 'EGRESO':
      return TIPOS_CALCULO_EGRESO
    case 'PROVISION':
      return TIPOS_CALCULO_PROVISION
    default:
      return []
  }
}

// Helper function to get label from value
export function getOptionLabel<T>(
  options: DropdownOption<T>[],
  value: T,
): string {
  const option = options.find((opt) => opt.value === value)
  return option ? option.label : String(value)
}

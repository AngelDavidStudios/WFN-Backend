import { personaFacade } from './facades/personaFacade'
import { empleadoFacade } from './facades/empleadoFacade'
import { departamentoFacade } from './facades/departamentoFacade'
import { nominaFacade } from './facades/nominaFacade'
import { novedadFacade } from './facades/novedadFacade'
import { parametroFacade } from './facades/parametroFacade'
import { bankingFacade } from './facades/bankingFacade'
import { provisionFacade } from './facades/provisionFacade'
import { workspaceFacade } from './facades/workspaceFacade'

/**
 * API Facade - Punto de entrada único para todas las operaciones de API
 *
 * Uso:
 * import { api } from '@/api'
 * const personas = await api.persona.getAll()
 * const empleado = await api.empleado.getById('123')
 */
export const api = {
  persona: personaFacade,
  empleado: empleadoFacade,
  departamento: departamentoFacade,
  nomina: nominaFacade,
  novedad: novedadFacade,
  parametro: parametroFacade,
  banking: bankingFacade,
  provision: provisionFacade,
  workspace: workspaceFacade,
}

// Also export individual facades for direct import
export {
  personaFacade,
  empleadoFacade,
  departamentoFacade,
  nominaFacade,
  novedadFacade,
  parametroFacade,
  bankingFacade,
  provisionFacade,
  workspaceFacade,
}

import axiosInstance from '../axiosInstance'
import type { Nomina, GenerarNominaRequest, RecalcularNominaRequest } from '@/types'

const BASE_URL = '/nomina'

export const nominaFacade = {
  /**
   * Obtiene todas las nóminas de un empleado
   */
  async getByEmpleado(empleadoId: string): Promise<Nomina[]> {
    const response = await axiosInstance.get<Nomina[]>(`${BASE_URL}/empleado/${empleadoId}`)
    return response.data
  },

  /**
   * Obtiene la nómina de un empleado en un periodo específico
   */
  async getByPeriodo(empleadoId: string, periodo: string): Promise<Nomina> {
    const response = await axiosInstance.get<Nomina>(
      `${BASE_URL}/empleado/${empleadoId}/periodo/${periodo}`,
    )
    return response.data
  },

  /**
   * Obtiene todas las nóminas de un periodo (todos los empleados)
   */
  async getByPeriodoGlobal(periodo: string): Promise<Nomina[]> {
    const response = await axiosInstance.get<Nomina[]>(`${BASE_URL}/periodo/${periodo}`)
    return response.data
  },

  /**
   * Genera una nueva nómina para un empleado en un periodo
   */
  async generar(data: GenerarNominaRequest): Promise<Nomina> {
    const response = await axiosInstance.post<Nomina>(`${BASE_URL}/generar`, data)
    return response.data
  },

  /**
   * Recalcula una nómina existente
   */
  async recalcular(data: RecalcularNominaRequest): Promise<Nomina> {
    const response = await axiosInstance.post<Nomina>(`${BASE_URL}/recalcular`, data)
    return response.data
  },

  /**
   * Elimina una nómina
   */
  async delete(empleadoId: string, periodo: string): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(
      `${BASE_URL}/empleado/${empleadoId}/periodo/${periodo}`,
    )
    return response.data
  },
}

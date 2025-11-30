import axiosInstance from '../axiosInstance'
import type { Novedad, NovedadCreateDTO, NovedadUpdateDTO } from '@/types'

const BASE_URL = '/novedad'

export const novedadFacade = {
  /**
   * Obtiene todas las novedades de un empleado
   */
  async getByEmpleado(empleadoId: string): Promise<Novedad[]> {
    const response = await axiosInstance.get<Novedad[]>(`${BASE_URL}/empleado/${empleadoId}`)
    return response.data
  },

  /**
   * Obtiene las novedades de un empleado en un periodo específico
   */
  async getByPeriodo(empleadoId: string, periodo: string): Promise<Novedad[]> {
    const response = await axiosInstance.get<Novedad[]>(
      `${BASE_URL}/empleado/${empleadoId}/periodo/${periodo}`,
    )
    return response.data
  },

  /**
   * Obtiene una novedad específica
   */
  async getById(empleadoId: string, periodo: string, novedadId: string): Promise<Novedad> {
    const response = await axiosInstance.get<Novedad>(
      `${BASE_URL}/empleado/${empleadoId}/periodo/${periodo}/${novedadId}`,
    )
    return response.data
  },

  /**
   * Crea una nueva novedad para un empleado
   */
  async create(empleadoId: string, data: NovedadCreateDTO): Promise<Novedad> {
    const response = await axiosInstance.post<Novedad>(`${BASE_URL}/empleado/${empleadoId}`, data)
    return response.data
  },

  /**
   * Actualiza una novedad existente
   */
  async update(
    empleadoId: string,
    periodo: string,
    novedadId: string,
    data: NovedadUpdateDTO,
  ): Promise<Novedad> {
    const response = await axiosInstance.put<Novedad>(
      `${BASE_URL}/empleado/${empleadoId}/periodo/${periodo}/${novedadId}`,
      data,
    )
    return response.data
  },

  /**
   * Elimina una novedad
   */
  async delete(empleadoId: string, periodo: string, novedadId: string): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(
      `${BASE_URL}/empleado/${empleadoId}/periodo/${periodo}/${novedadId}`,
    )
    return response.data
  },
}

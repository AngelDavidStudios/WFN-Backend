import axiosInstance from '../axiosInstance'
import type {
  Provision,
  ProvisionCreateDTO,
  ProvisionUpdateDTO,
  TipoProvision,
  ProcesarProvisionesRequest,
} from '@/types'

const BASE_URL = '/provision'

export const provisionFacade = {
  /**
   * Obtiene todas las provisiones de un empleado
   */
  async getByEmpleado(empleadoId: string): Promise<Provision[]> {
    const response = await axiosInstance.get<Provision[]>(`${BASE_URL}/empleado/${empleadoId}`)
    return response.data
  },

  /**
   * Obtiene las provisiones de un empleado en un periodo específico
   */
  async getByPeriodo(empleadoId: string, periodo: string): Promise<Provision[]> {
    const response = await axiosInstance.get<Provision[]>(
      `${BASE_URL}/empleado/${empleadoId}/periodo/${periodo}`,
    )
    return response.data
  },

  /**
   * Obtiene las provisiones de un empleado por tipo
   */
  async getByTipo(empleadoId: string, tipoProvision: TipoProvision): Promise<Provision[]> {
    const response = await axiosInstance.get<Provision[]>(
      `${BASE_URL}/empleado/${empleadoId}/tipo/${tipoProvision}`,
    )
    return response.data
  },

  /**
   * Obtiene una provisión específica
   */
  async getById(empleadoId: string, tipoProvision: TipoProvision, periodo: string): Promise<Provision> {
    const response = await axiosInstance.get<Provision>(
      `${BASE_URL}/empleado/${empleadoId}/tipo/${tipoProvision}/periodo/${periodo}`,
    )
    return response.data
  },

  /**
   * Crea una nueva provisión para un empleado
   */
  async create(empleadoId: string, data: ProvisionCreateDTO): Promise<Provision> {
    const response = await axiosInstance.post<Provision>(`${BASE_URL}/empleado/${empleadoId}`, data)
    return response.data
  },

  /**
   * Actualiza una provisión existente
   */
  async update(
    empleadoId: string,
    tipoProvision: TipoProvision,
    periodo: string,
    data: ProvisionUpdateDTO,
  ): Promise<Provision> {
    const response = await axiosInstance.put<Provision>(
      `${BASE_URL}/empleado/${empleadoId}/tipo/${tipoProvision}/periodo/${periodo}`,
      data,
    )
    return response.data
  },

  /**
   * Elimina una provisión
   */
  async delete(
    empleadoId: string,
    tipoProvision: TipoProvision,
    periodo: string,
  ): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(
      `${BASE_URL}/empleado/${empleadoId}/tipo/${tipoProvision}/periodo/${periodo}`,
    )
    return response.data
  },

  /**
   * Procesa y calcula todas las provisiones de un empleado en un periodo
   */
  async procesar(data: ProcesarProvisionesRequest): Promise<{ message: string }> {
    const response = await axiosInstance.post<{ message: string }>(`${BASE_URL}/procesar`, data)
    return response.data
  },
}

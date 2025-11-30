import axiosInstance from '../axiosInstance'
import type { Parametro, ParametroCreateDTO, ParametroUpdateDTO, TipoParametro } from '@/types'

const BASE_URL = '/parametro'

export const parametroFacade = {
  /**
   * Obtiene todos los parámetros
   */
  async getAll(): Promise<Parametro[]> {
    const response = await axiosInstance.get<Parametro[]>(BASE_URL)
    return response.data
  },

  /**
   * Obtiene un parámetro por ID
   */
  async getById(id: string): Promise<Parametro> {
    const response = await axiosInstance.get<Parametro>(`${BASE_URL}/${id}`)
    return response.data
  },

  /**
   * Obtiene parámetros por tipo (INGRESO, EGRESO, PROVISION)
   */
  async getByTipo(tipo: TipoParametro): Promise<Parametro[]> {
    const response = await axiosInstance.get<Parametro[]>(`${BASE_URL}/tipo/${tipo}`)
    return response.data
  },

  /**
   * Crea un nuevo parámetro
   */
  async create(data: ParametroCreateDTO): Promise<Parametro> {
    const response = await axiosInstance.post<Parametro>(BASE_URL, data)
    return response.data
  },

  /**
   * Actualiza un parámetro existente
   */
  async update(id: string, data: ParametroUpdateDTO): Promise<Parametro> {
    const response = await axiosInstance.put<Parametro>(`${BASE_URL}/${id}`, data)
    return response.data
  },

  /**
   * Elimina un parámetro
   */
  async delete(id: string): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(`${BASE_URL}/${id}`)
    return response.data
  },
}

import axiosInstance from '../axiosInstance'
import type { Departamento, DepartamentoCreateDTO, DepartamentoUpdateDTO } from '@/types'

const BASE_URL = '/departamento'

export const departamentoFacade = {
  /**
   * Obtiene todos los departamentos
   */
  async getAll(): Promise<Departamento[]> {
    const response = await axiosInstance.get<Departamento[]>(BASE_URL)
    return response.data
  },

  /**
   * Obtiene un departamento por ID
   */
  async getById(id: string): Promise<Departamento> {
    const response = await axiosInstance.get<Departamento>(`${BASE_URL}/${id}`)
    return response.data
  },

  /**
   * Crea un nuevo departamento
   */
  async create(data: DepartamentoCreateDTO): Promise<Departamento> {
    const response = await axiosInstance.post<Departamento>(BASE_URL, data)
    return response.data
  },

  /**
   * Actualiza un departamento existente
   */
  async update(id: string, data: DepartamentoUpdateDTO): Promise<Departamento> {
    const response = await axiosInstance.put<Departamento>(`${BASE_URL}/${id}`, data)
    return response.data
  },

  /**
   * Elimina un departamento
   */
  async delete(id: string): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(`${BASE_URL}/${id}`)
    return response.data
  },
}

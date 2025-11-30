import axiosInstance from '../axiosInstance'
import type { Empleado, EmpleadoCreateDTO, EmpleadoUpdateDTO } from '@/types'

const BASE_URL = '/empleado'

export const empleadoFacade = {
  /**
   * Obtiene todos los empleados
   */
  async getAll(): Promise<Empleado[]> {
    const response = await axiosInstance.get<Empleado[]>(BASE_URL)
    return response.data
  },

  /**
   * Obtiene un empleado por ID
   */
  async getById(id: string): Promise<Empleado> {
    const response = await axiosInstance.get<Empleado>(`${BASE_URL}/${id}`)
    return response.data
  },

  /**
   * Crea un nuevo empleado
   */
  async create(data: EmpleadoCreateDTO): Promise<Empleado> {
    const response = await axiosInstance.post<Empleado>(BASE_URL, data)
    return response.data
  },

  /**
   * Actualiza un empleado existente
   */
  async update(id: string, data: EmpleadoUpdateDTO): Promise<Empleado> {
    const response = await axiosInstance.put<Empleado>(`${BASE_URL}/${id}`, data)
    return response.data
  },

  /**
   * Elimina un empleado
   */
  async delete(id: string): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(`${BASE_URL}/${id}`)
    return response.data
  },
}

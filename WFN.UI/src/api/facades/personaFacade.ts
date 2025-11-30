import axiosInstance from '../axiosInstance'
import type { Persona, PersonaCreateDTO, PersonaUpdateDTO } from '@/types'

const BASE_URL = '/persona'

export const personaFacade = {
  /**
   * Obtiene todas las personas
   */
  async getAll(): Promise<Persona[]> {
    const response = await axiosInstance.get<Persona[]>(BASE_URL)
    return response.data
  },

  /**
   * Obtiene una persona por ID
   */
  async getById(id: string): Promise<Persona> {
    const response = await axiosInstance.get<Persona>(`${BASE_URL}/${id}`)
    return response.data
  },

  /**
   * Crea una nueva persona
   */
  async create(data: PersonaCreateDTO): Promise<Persona> {
    const response = await axiosInstance.post<Persona>(BASE_URL, data)
    return response.data
  },

  /**
   * Actualiza una persona existente
   */
  async update(id: string, data: PersonaUpdateDTO): Promise<Persona> {
    const response = await axiosInstance.put<Persona>(`${BASE_URL}/${id}`, data)
    return response.data
  },

  /**
   * Elimina una persona
   */
  async delete(id: string): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(`${BASE_URL}/${id}`)
    return response.data
  },
}

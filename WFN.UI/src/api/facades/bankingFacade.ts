import axiosInstance from '../axiosInstance'
import type { Banking, BankingCreateDTO, BankingUpdateDTO } from '@/types'

const BASE_URL = '/banking'

export const bankingFacade = {
  /**
   * Obtiene todas las cuentas bancarias de una persona
   */
  async getByPersona(personaId: string): Promise<Banking[]> {
    const response = await axiosInstance.get<Banking[]>(`${BASE_URL}/persona/${personaId}`)
    return response.data
  },

  /**
   * Obtiene una cuenta bancaria específica
   */
  async getById(personaId: string, bankId: string): Promise<Banking> {
    const response = await axiosInstance.get<Banking>(`${BASE_URL}/persona/${personaId}/${bankId}`)
    return response.data
  },

  /**
   * Crea una nueva cuenta bancaria para una persona
   */
  async create(personaId: string, data: BankingCreateDTO): Promise<Banking> {
    const response = await axiosInstance.post<Banking>(`${BASE_URL}/persona/${personaId}`, data)
    return response.data
  },

  /**
   * Actualiza una cuenta bancaria existente
   */
  async update(personaId: string, bankId: string, data: BankingUpdateDTO): Promise<Banking> {
    const response = await axiosInstance.put<Banking>(
      `${BASE_URL}/persona/${personaId}/${bankId}`,
      data,
    )
    return response.data
  },

  /**
   * Elimina una cuenta bancaria
   */
  async delete(personaId: string, bankId: string): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(
      `${BASE_URL}/persona/${personaId}/${bankId}`,
    )
    return response.data
  },
}

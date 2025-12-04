import axiosInstance from '../axiosInstance'
import type { WorkspaceNomina, WorkspaceEstado } from '@/types'

const BASE_URL = '/workspace'

export const workspaceFacade = {
  /**
   * Obtiene todos los workspaces
   */
  async getAll(): Promise<WorkspaceNomina[]> {
    const response = await axiosInstance.get<WorkspaceNomina[]>(BASE_URL)
    return response.data
  },

  /**
   * Obtiene un workspace por periodo
   */
  async getByPeriodo(periodo: string): Promise<WorkspaceNomina> {
    const response = await axiosInstance.get<WorkspaceNomina>(`${BASE_URL}/${periodo}`)
    return response.data
  },

  /**
   * Crea un nuevo workspace para un periodo
   */
  async crearPeriodo(periodo: string): Promise<WorkspaceNomina> {
    const response = await axiosInstance.post<WorkspaceNomina>(`${BASE_URL}/${periodo}`)
    return response.data
  },

  /**
   * Cierra un periodo
   */
  async cerrarPeriodo(periodo: string): Promise<WorkspaceNomina> {
    const response = await axiosInstance.post<WorkspaceNomina>(`${BASE_URL}/${periodo}/cerrar`)
    return response.data
  },

  /**
   * Actualiza un workspace
   */
  async update(workspace: WorkspaceNomina): Promise<WorkspaceNomina> {
    const response = await axiosInstance.put<WorkspaceNomina>(BASE_URL, workspace)
    return response.data
  },

  /**
   * Elimina un workspace
   */
  async delete(periodo: string): Promise<{ message: string }> {
    const response = await axiosInstance.delete<{ message: string }>(`${BASE_URL}/${periodo}`)
    return response.data
  },

  /**
   * Verifica el estado de un periodo
   */
  async verificarEstado(periodo: string): Promise<WorkspaceEstado> {
    const response = await axiosInstance.get<WorkspaceEstado>(`${BASE_URL}/${periodo}/estado`)
    return response.data
  },
}

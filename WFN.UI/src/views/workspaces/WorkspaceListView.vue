<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { PlusIcon, LockClosedIcon, LockOpenIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { WorkspaceNomina } from '@/types'
import { EstadoWorkspace } from '@/types'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import FormInput from '@/components/forms/FormInput.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const uiStore = useUIStore()

const loading = ref(true)
const workspaces = ref<WorkspaceNomina[]>([])

// Create modal
const createModalOpen = ref(false)
const newPeriodo = ref('')
const creating = ref(false)

// Close modal
const closeModalOpen = ref(false)
const workspaceToClose = ref<WorkspaceNomina | null>(null)
const closing = ref(false)

// Delete modal
const deleteModalOpen = ref(false)
const workspaceToDelete = ref<WorkspaceNomina | null>(null)
const deleting = ref(false)

const columns = [
  { key: 'periodo', label: 'Periodo', width: '150px' },
  { key: 'estadoDisplay', label: 'Estado', width: '120px' },
  { key: 'fechaCreacion', label: 'Fecha Apertura', width: '180px' },
  { key: 'fechaCierre', label: 'Fecha Cierre', width: '180px' },
]

async function loadWorkspaces() {
  loading.value = true
  try {
    const data = await api.workspace.getAll()
    workspaces.value = data.map((w) => ({
      ...w,
      estadoDisplay: w.estado === EstadoWorkspace.Abierto ? 'Abierto' : 'Cerrado',
      fechaCreacionDisplay: w.fechaCreacion || '-',
      fechaCierreDisplay: w.fechaCierre || '-',
    }))
  } catch (error) {
    console.error('Error loading workspaces:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar los periodos')
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  const now = new Date()
  newPeriodo.value = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`
  createModalOpen.value = true
}

async function handleCreate() {
  if (!newPeriodo.value) {
    uiStore.notifyWarning('Atención', 'Ingrese un periodo válido')
    return
  }

  creating.value = true
  try {
    await api.workspace.crearPeriodo(newPeriodo.value)
    uiStore.notifySuccess('Éxito', `Periodo ${newPeriodo.value} creado correctamente`)
    createModalOpen.value = false
    await loadWorkspaces()
  } catch (error) {
    console.error('Error creating workspace:', error)
    uiStore.notifyError('Error', 'No se pudo crear el periodo')
  } finally {
    creating.value = false
  }
}

function confirmClose(workspace: WorkspaceNomina) {
  workspaceToClose.value = workspace
  closeModalOpen.value = true
}

async function handleClose() {
  if (!workspaceToClose.value) return

  closing.value = true
  try {
    await api.workspace.cerrarPeriodo(workspaceToClose.value.periodo)
    uiStore.notifySuccess('Éxito', `Periodo ${workspaceToClose.value.periodo} cerrado correctamente`)
    closeModalOpen.value = false
    await loadWorkspaces()
  } catch (error) {
    console.error('Error closing workspace:', error)
    uiStore.notifyError('Error', 'No se pudo cerrar el periodo')
  } finally {
    closing.value = false
    workspaceToClose.value = null
  }
}

function confirmDelete(workspace: WorkspaceNomina) {
  workspaceToDelete.value = workspace
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!workspaceToDelete.value) return

  deleting.value = true
  try {
    await api.workspace.delete(workspaceToDelete.value.periodo)
    uiStore.notifySuccess('Éxito', `Periodo ${workspaceToDelete.value.periodo} eliminado`)
    deleteModalOpen.value = false
    await loadWorkspaces()
  } catch (error) {
    console.error('Error deleting workspace:', error)
    uiStore.notifyError('Error', 'No se pudo eliminar el periodo')
  } finally {
    deleting.value = false
    workspaceToDelete.value = null
  }
}

onMounted(() => {
  loadWorkspaces()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Workspaces</h1>
        <p class="mt-1 text-sm text-gray-500">Gestión de periodos de nómina</p>
      </div>
      <BaseButton variant="primary" @click="openCreateModal">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nuevo Periodo
      </BaseButton>
    </div>

    <!-- Info -->
    <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
      <h3 class="font-medium text-blue-800">Información sobre Workspaces</h3>
      <ul class="mt-2 text-sm text-blue-700 list-disc list-inside space-y-1">
        <li>Un periodo <strong>ABIERTO</strong> permite crear y modificar nóminas y novedades</li>
        <li>Un periodo <strong>CERRADO</strong> no permite modificaciones</li>
        <li>Solo se puede cerrar un periodo, no se puede reabrir</li>
        <li>Eliminar un periodo eliminará todos los datos asociados</li>
      </ul>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Table -->
    <DataTable
      v-else
      :columns="columns"
      :data="workspaces as unknown as Record<string, unknown>[]"
      empty-text="No hay periodos registrados"
    >
      <template #cell-estadoDisplay="{ value, row }">
        <span
          :class="[
            'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
            (row as unknown as WorkspaceNomina).estado === EstadoWorkspace.Abierto
              ? 'bg-green-100 text-green-800'
              : 'bg-gray-100 text-gray-800',
          ]"
        >
          <component
            :is="(row as unknown as WorkspaceNomina).estado === EstadoWorkspace.Abierto ? LockOpenIcon : LockClosedIcon"
            class="h-3 w-3 mr-1"
          />
          {{ value }}
        </span>
      </template>
      <template #actions="{ row }">
        <div class="flex items-center space-x-2">
          <button
            v-if="(row as unknown as WorkspaceNomina).estado === EstadoWorkspace.Abierto"
            type="button"
            class="text-gray-400 hover:text-orange-600"
            title="Cerrar Periodo"
            @click.stop="confirmClose(row as unknown as WorkspaceNomina)"
          >
            <LockClosedIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-red-600"
            title="Eliminar"
            @click.stop="confirmDelete(row as unknown as WorkspaceNomina)"
          >
            <TrashIcon class="h-5 w-5" />
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Create Modal -->
    <BaseModal :open="createModalOpen" title="Crear Nuevo Periodo" @close="createModalOpen = false">
      <div class="space-y-4">
        <p class="text-gray-500">Ingrese el periodo a crear en formato YYYY-MM</p>
        <FormInput
          v-model="newPeriodo"
          type="month"
          label="Periodo"
          required
        />
      </div>
      <template #footer>
        <BaseButton variant="outline" @click="createModalOpen = false">Cancelar</BaseButton>
        <BaseButton variant="primary" :loading="creating" @click="handleCreate">
          Crear Periodo
        </BaseButton>
      </template>
    </BaseModal>

    <!-- Close Modal -->
    <BaseModal :open="closeModalOpen" title="Cerrar Periodo" @close="closeModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea cerrar el periodo
        <strong>{{ workspaceToClose?.periodo }}</strong>?
        Esta acción no se puede deshacer y no podrá modificar las nóminas de este periodo.
      </p>
      <template #footer>
        <BaseButton variant="outline" @click="closeModalOpen = false">Cancelar</BaseButton>
        <BaseButton variant="danger" :loading="closing" @click="handleClose">
          Cerrar Periodo
        </BaseButton>
      </template>
    </BaseModal>

    <!-- Delete Modal -->
    <BaseModal :open="deleteModalOpen" title="Eliminar Periodo" @close="deleteModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea eliminar el periodo
        <strong>{{ workspaceToDelete?.periodo }}</strong>?
        Se eliminarán todas las nóminas y novedades asociadas. Esta acción no se puede deshacer.
      </p>
      <template #footer>
        <BaseButton variant="outline" @click="deleteModalOpen = false">Cancelar</BaseButton>
        <BaseButton variant="danger" :loading="deleting" @click="handleDelete">
          Eliminar
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

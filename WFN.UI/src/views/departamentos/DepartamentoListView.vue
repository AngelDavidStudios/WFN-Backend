<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { PlusIcon, PencilIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Departamento } from '@/types'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'

const router = useRouter()
const uiStore = useUIStore()

const loading = ref(true)
const departamentos = ref<Departamento[]>([])
const deleteModalOpen = ref(false)
const departamentoToDelete = ref<Departamento | null>(null)
const deleting = ref(false)

const columns = [
  { key: 'nombre', label: 'Nombre' },
  { key: 'ubicacion', label: 'Ubicación' },
  { key: 'cargo', label: 'Cargo' },
  { key: 'centroCosto', label: 'Centro de Costo', width: '150px' },
  { key: 'email', label: 'Email' },
  { key: 'telefono', label: 'Teléfono', width: '150px' },
]

async function loadDepartamentos() {
  loading.value = true
  try {
    departamentos.value = await api.departamento.getAll()
  } catch (error) {
    console.error('Error loading departamentos:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar los departamentos')
  } finally {
    loading.value = false
  }
}

function goToNew() {
  router.push('/departamentos/nuevo')
}

function goToEdit(departamento: Departamento) {
  console.log('Editando departamento:', departamento)
  const id = departamento.id_Departamento
  if (!id) {
    console.error('ID de departamento no encontrado:', departamento)
    uiStore.notifyError('Error', 'No se pudo obtener el ID del departamento')
    return
  }
  router.push(`/departamentos/${id}/editar`)
}

function confirmDelete(departamento: Departamento) {
  console.log('Confirmando eliminación de departamento:', departamento)
  const id = departamento.id_Departamento
  if (!id) {
    console.error('ID de departamento no encontrado:', departamento)
    uiStore.notifyError('Error', 'No se pudo obtener el ID del departamento')
    return
  }
  departamentoToDelete.value = departamento
  deleteModalOpen.value = true
}

// Helper functions for table actions
function handleEdit(row: Record<string, unknown>) {
  const dept = row as unknown as Departamento
  goToEdit(dept)
}

function handleDeleteClick(row: Record<string, unknown>) {
  const dept = row as unknown as Departamento
  confirmDelete(dept)
}

async function handleDelete() {
  if (!departamentoToDelete.value) return

  deleting.value = true
  try {
    await api.departamento.delete(departamentoToDelete.value.id_Departamento)
    uiStore.notifySuccess('Éxito', 'Departamento eliminado correctamente')
    deleteModalOpen.value = false
    await loadDepartamentos()
  } catch (error) {
    console.error('Error deleting departamento:', error)
    uiStore.notifyError('Error', 'No se pudo eliminar el departamento')
  } finally {
    deleting.value = false
    departamentoToDelete.value = null
  }
}

onMounted(() => {
  loadDepartamentos()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Departamentos</h1>
        <p class="mt-1 text-sm text-gray-500">Gestión de departamentos de la empresa</p>
      </div>
      <BaseButton variant="primary" @click="goToNew">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nuevo Departamento
      </BaseButton>
    </div>

    <!-- Table -->
    <DataTable
      :columns="columns"
      :data="departamentos as unknown as Record<string, unknown>[]"
      :loading="loading"
      empty-text="No hay departamentos registrados"
    >
      <template #actions="{ row }">
        <div class="flex items-center justify-end space-x-3" @click.stop>
          <button
            type="button"
            class="inline-flex items-center p-1.5 rounded-md text-gray-600 hover:text-white hover:bg-primary-600 transition-all duration-150"
            title="Editar departamento"
            @click="handleEdit(row)"
          >
            <PencilIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="inline-flex items-center p-1.5 rounded-md text-gray-600 hover:text-white hover:bg-red-600 transition-all duration-150"
            title="Eliminar departamento"
            @click="handleDeleteClick(row)"
          >
            <TrashIcon class="h-5 w-5" />
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Delete Confirmation Modal -->
    <BaseModal :open="deleteModalOpen" title="Confirmar Eliminación" @close="deleteModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea eliminar el departamento
        <strong>{{ departamentoToDelete?.nombre }}</strong>?
        Esta acción no se puede deshacer.
      </p>
      <template #footer>
        <BaseButton variant="outline" @click="deleteModalOpen = false"> Cancelar </BaseButton>
        <BaseButton variant="danger" :loading="deleting" @click="handleDelete">
          Eliminar
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

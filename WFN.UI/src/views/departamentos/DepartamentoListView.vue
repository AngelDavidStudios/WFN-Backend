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
  router.push(`/departamentos/${departamento.id_Departamento}/editar`)
}

function confirmDelete(departamento: Departamento) {
  departamentoToDelete.value = departamento
  deleteModalOpen.value = true
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
        <div class="flex items-center space-x-2">
          <button
            type="button"
            class="text-gray-400 hover:text-primary-600"
            title="Editar"
            @click.stop="goToEdit(row as unknown as Departamento)"
          >
            <PencilIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-red-600"
            title="Eliminar"
            @click.stop="confirmDelete(row as unknown as Departamento)"
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

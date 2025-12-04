<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { PlusIcon, PencilIcon, TrashIcon, EyeIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Persona } from '@/types'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'

const router = useRouter()
const uiStore = useUIStore()

const loading = ref(true)
const personas = ref<Persona[]>([])
const deleteModalOpen = ref(false)
const personaToDelete = ref<Persona | null>(null)
const deleting = ref(false)

const columns = [
  { key: 'dni', label: 'DNI', width: '120px' },
  { key: 'nombreCompleto', label: 'Nombre Completo' },
  { key: 'genderDisplay', label: 'Género', width: '100px' },
  { key: 'edad', label: 'Edad', width: '80px', align: 'center' as const },
  { key: 'correoDisplay', label: 'Correo' },
  { key: 'telefonoDisplay', label: 'Teléfono', width: '150px' },
]

async function loadPersonas() {
  loading.value = true
  try {
    const data = await api.persona.getAll()
    personas.value = data.map((p) => ({
      ...p,
      nombreCompleto: [p.primerNombre, p.segundoNombre, p.apellidoPaterno, p.apellidoMaterno]
        .filter(Boolean)
        .join(' '),
      genderDisplay: p.gender === 'M' ? 'Masculino' : p.gender === 'F' ? 'Femenino' : 'Otro',
      correoDisplay: p.correo?.[0] || '-',
      telefonoDisplay: p.telefono?.[0] || '-',
    }))
  } catch (error: any) {
    console.error('Error loading personas:', error)
    const errorMessage = error.response?.data?.message || 'No se pudieron cargar las personas'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    loading.value = false
  }
}

function goToNew() {
  router.push('/personas/nuevo')
}

function goToEdit(persona: Persona) {
  router.push(`/personas/${persona.id_Persona}/editar`)
}

function goToView(persona: Persona) {
  router.push(`/personas/${persona.id_Persona}`)
}

function confirmDelete(persona: Persona) {
  personaToDelete.value = persona
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!personaToDelete.value) return

  deleting.value = true
  try {
    await api.persona.delete(personaToDelete.value.id_Persona)
    uiStore.notifySuccess('Éxito', 'Persona eliminada correctamente')
    deleteModalOpen.value = false
    await loadPersonas()
  } catch (error: any) {
    console.error('Error deleting persona:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo eliminar la persona'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    deleting.value = false
    personaToDelete.value = null
  }
}

function handleRowClick(row: Record<string, unknown>) {
  const persona = row as unknown as Persona
  goToView(persona)
}

onMounted(() => {
  loadPersonas()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Personas</h1>
        <p class="mt-1 text-sm text-gray-500">Gestión de personas del sistema</p>
      </div>
      <BaseButton variant="primary" @click="goToNew">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nueva Persona
      </BaseButton>
    </div>

    <!-- Table -->
    <DataTable
      :columns="columns"
      :data="personas as unknown as Record<string, unknown>[]"
      :loading="loading"
      empty-text="No hay personas registradas"
      @row-click="handleRowClick"
    >
      <template #actions="{ row }">
        <div class="flex items-center space-x-2">
          <button
            type="button"
            class="text-gray-400 hover:text-primary-600"
            title="Ver"
            @click.stop="goToView(row as unknown as Persona)"
          >
            <EyeIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-primary-600"
            title="Editar"
            @click.stop="goToEdit(row as unknown as Persona)"
          >
            <PencilIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-red-600"
            title="Eliminar"
            @click.stop="confirmDelete(row as unknown as Persona)"
          >
            <TrashIcon class="h-5 w-5" />
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Delete Confirmation Modal -->
    <BaseModal :open="deleteModalOpen" title="Confirmar Eliminación" @close="deleteModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea eliminar a
        <strong>{{ personaToDelete?.primerNombre }} {{ personaToDelete?.apellidoPaterno }}</strong>?
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

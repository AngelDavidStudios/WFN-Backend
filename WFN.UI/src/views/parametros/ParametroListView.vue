<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { PlusIcon, PencilIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Parametro } from '@/types'
import { TIPO_PARAMETRO_OPTIONS, getTipoCalculoOptions, getOptionLabel } from '@/constants'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import FormInput from '@/components/forms/FormInput.vue'
import FormSelect from '@/components/forms/FormSelect.vue'
import FormTextarea from '@/components/forms/FormTextarea.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const uiStore = useUIStore()

const loading = ref(true)
const parametros = ref<Parametro[]>([])

// Form modal
const formModalOpen = ref(false)
const isEditing = ref(false)
const saving = ref(false)
const parametroForm = ref({
  nombre: '',
  tipo: 'INGRESO' as 'INGRESO' | 'EGRESO' | 'PROVISION',
  tipoCalculo: '',
  descripcion: '',
})
const editingParametroId = ref('')

// Delete modal
const deleteModalOpen = ref(false)
const parametroToDelete = ref<Parametro | null>(null)
const deleting = ref(false)

const columns = [
  { key: 'nombre', label: 'Nombre' },
  { key: 'tipo', label: 'Tipo', width: '120px' },
  { key: 'tipoCalculo', label: 'Tipo de Cálculo' },
  { key: 'descripcion', label: 'Descripción' },
]

async function loadParametros() {
  loading.value = true
  try {
    parametros.value = await api.parametro.getAll()
  } catch (error) {
    console.error('Error loading parametros:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar los parámetros')
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  isEditing.value = false
  parametroForm.value = {
    nombre: '',
    tipo: 'INGRESO',
    tipoCalculo: '',
    descripcion: '',
  }
  formModalOpen.value = true
}

function openEditModal(parametro: Parametro) {
  isEditing.value = true
  editingParametroId.value = parametro.id_Parametro
  parametroForm.value = {
    nombre: parametro.nombre,
    tipo: parametro.tipo as 'INGRESO' | 'EGRESO' | 'PROVISION',
    tipoCalculo: parametro.tipoCalculo,
    descripcion: parametro.descripcion,
  }
  formModalOpen.value = true
}

async function handleSave() {
  if (!parametroForm.value.nombre || !parametroForm.value.tipoCalculo) {
    uiStore.notifyWarning('Atención', 'Complete todos los campos requeridos')
    return
  }

  saving.value = true
  try {
    if (isEditing.value) {
      await api.parametro.update(editingParametroId.value, {
        ...parametroForm.value,
        id_Parametro: editingParametroId.value,
      })
      uiStore.notifySuccess('Éxito', 'Parámetro actualizado correctamente')
    } else {
      await api.parametro.create(parametroForm.value)
      uiStore.notifySuccess('Éxito', 'Parámetro creado correctamente')
    }
    formModalOpen.value = false
    await loadParametros()
  } catch (error) {
    console.error('Error saving parametro:', error)
    uiStore.notifyError('Error', 'No se pudo guardar el parámetro')
  } finally {
    saving.value = false
  }
}

function confirmDelete(parametro: Parametro) {
  parametroToDelete.value = parametro
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!parametroToDelete.value) return

  deleting.value = true
  try {
    await api.parametro.delete(parametroToDelete.value.id_Parametro)
    uiStore.notifySuccess('Éxito', 'Parámetro eliminado correctamente')
    deleteModalOpen.value = false
    await loadParametros()
  } catch (error) {
    console.error('Error deleting parametro:', error)
    uiStore.notifyError('Error', 'No se pudo eliminar el parámetro')
  } finally {
    deleting.value = false
    parametroToDelete.value = null
  }
}

onMounted(() => {
  loadParametros()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Parámetros</h1>
        <p class="mt-1 text-sm text-gray-500">Configuración de parámetros de nómina</p>
      </div>
      <BaseButton variant="primary" @click="openCreateModal">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nuevo Parámetro
      </BaseButton>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Table -->
    <DataTable
      v-else
      :columns="columns"
      :data="parametros as unknown as Record<string, unknown>[]"
      empty-text="No hay parámetros registrados"
    >
      <template #cell-tipo="{ value }">
        <span
          :class="[
            'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
            value === 'INGRESO'
              ? 'bg-green-100 text-green-800'
              : value === 'EGRESO'
                ? 'bg-red-100 text-red-800'
                : 'bg-blue-100 text-blue-800',
          ]"
        >
          {{ getOptionLabel(TIPO_PARAMETRO_OPTIONS, value as string) }}
        </span>
      </template>
      <template #actions="{ row }">
        <div class="flex items-center space-x-2">
          <button
            type="button"
            class="text-gray-400 hover:text-primary-600"
            title="Editar"
            @click.stop="openEditModal(row as unknown as Parametro)"
          >
            <PencilIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-red-600"
            title="Eliminar"
            @click.stop="confirmDelete(row as unknown as Parametro)"
          >
            <TrashIcon class="h-5 w-5" />
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Form Modal -->
    <BaseModal
      :open="formModalOpen"
      :title="isEditing ? 'Editar Parámetro' : 'Nuevo Parámetro'"
      size="lg"
      @close="formModalOpen = false"
    >
      <div class="space-y-4">
        <FormInput
          v-model="parametroForm.nombre"
          label="Nombre"
          placeholder="HORAS_EXTRAS_50"
          required
        />
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <FormSelect
            v-model="parametroForm.tipo"
            label="Tipo"
            :options="TIPO_PARAMETRO_OPTIONS"
            required
          />
          <FormSelect
            v-model="parametroForm.tipoCalculo"
            label="Tipo de Cálculo"
            :options="getTipoCalculoOptions(parametroForm.tipo)"
            placeholder="Seleccione tipo de cálculo"
            required
          />
        </div>
        <FormTextarea
          v-model="parametroForm.descripcion"
          label="Descripción"
          placeholder="Descripción del parámetro"
          :rows="3"
        />
      </div>
      <template #footer>
        <BaseButton variant="outline" @click="formModalOpen = false">Cancelar</BaseButton>
        <BaseButton variant="primary" :loading="saving" @click="handleSave">
          {{ isEditing ? 'Actualizar' : 'Crear' }}
        </BaseButton>
      </template>
    </BaseModal>

    <!-- Delete Modal -->
    <BaseModal :open="deleteModalOpen" title="Confirmar Eliminación" @close="deleteModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea eliminar el parámetro
        <strong>{{ parametroToDelete?.nombre }}</strong>?
        Esta acción no se puede deshacer.
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

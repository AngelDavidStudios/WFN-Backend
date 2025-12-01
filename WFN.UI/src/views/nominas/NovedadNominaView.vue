<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ArrowLeftIcon, PlusIcon, PencilIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Novedad, Empleado, Parametro } from '@/types'
import {
  TIPO_NOVEDAD_OPTIONS,
  TIPOS_CALCULO_INGRESO,
  TIPOS_CALCULO_EGRESO,
  getOptionLabel
} from '@/constants'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import FormSelect from '@/components/forms/FormSelect.vue'
import FormInput from '@/components/forms/FormInput.vue'
import FormTextarea from '@/components/forms/FormTextarea.vue'
import FormCheckbox from '@/components/forms/FormCheckbox.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const periodo = computed(() => route.params.periodo as string)
const empleadoId = computed(() => route.params.empleadoId as string)

const loading = ref(true)
const novedades = ref<Novedad[]>([])
const empleado = ref<Empleado | null>(null)
const persona = ref<any>(null)
const parametros = ref<Parametro[]>([])

// Form modal
const formModalOpen = ref(false)
const isEditing = ref(false)
const saving = ref(false)
const novedadForm = ref({
  id_Parametro: '',
  tipoNovedad: 'INGRESO' as 'INGRESO' | 'EGRESO' | 'PROVISION',
  descripcion: '',
  montoAplicado: 0,
  cantidadHoras: 0,
  is_Gravable: true,
})
const editingNovedadId = ref('')
const errors = ref<Record<string, string>>({})

// Delete modal
const deleteModalOpen = ref(false)
const novedadToDelete = ref<Novedad | null>(null)
const deleting = ref(false)

const columns = [
  { key: 'tipoNovedad', label: 'Tipo', width: '100px' },
  { key: 'nombreParametro', label: 'Concepto' },
  { key: 'descripcion', label: 'Descripción' },
  { key: 'montoAplicado', label: 'Monto', align: 'right' as const },
  { key: 'is_Gravable', label: 'Gravable', width: '80px', align: 'center' as const },
]

const ingresos = computed(() => novedades.value.filter(n => n.tipoNovedad === 'INGRESO'))
const egresos = computed(() => novedades.value.filter(n => n.tipoNovedad === 'EGRESO'))

const parametrosDisponibles = computed(() => {
  if (novedadForm.value.tipoNovedad === 'INGRESO') {
    return parametros.value
      .filter(p => p.tipo === 'INGRESO')
      .map(p => ({ value: p.id_Parametro, label: `${p.nombre}` }))
  } else if (novedadForm.value.tipoNovedad === 'EGRESO') {
    return parametros.value
      .filter(p => p.tipo === 'EGRESO')
      .map(p => ({ value: p.id_Parametro, label: `${p.nombre}` }))
  }
  return []
})

const empleadoNombre = computed(() => {
  if (!persona.value) return ''
  return [
    persona.value.primerNombre,
    persona.value.segundoNombre,
    persona.value.apellidoPaterno,
    persona.value.apellidoMaterno
  ].filter(Boolean).join(' ')
})

async function loadData() {
  loading.value = true
  try {
    // Cargar empleado
    const empleadoData = await api.empleado.getById(empleadoId.value)
    empleado.value = empleadoData

    // Cargar persona
    const personaData = await api.persona.getById(empleadoData.id_Persona)
    persona.value = personaData

    // Cargar parámetros
    const parametrosData = await api.parametro.getAll()
    parametros.value = parametrosData

    // Cargar novedades
    await loadNovedades()
  } catch (error: any) {
    console.error('Error loading data:', error)
    const errorMessage = error.response?.data?.message || 'No se pudieron cargar los datos'
    uiStore.notifyError('Error', errorMessage)
    router.push(`/nominas/periodo/${periodo.value}`)
  } finally {
    loading.value = false
  }
}

async function loadNovedades() {
  try {
    const data = await api.novedad.getByPeriodo(empleadoId.value, periodo.value)

    // Agregar nombre del parámetro a cada novedad
    novedades.value = data.map((n: Novedad) => ({
      ...n,
      nombreParametro: parametros.value.find(p => p.id_Parametro === n.id_Parametro)?.nombre || n.id_Parametro
    }))
  } catch (error: any) {
    if (error.response?.status === 404) {
      novedades.value = []
    } else {
      console.error('Error loading novedades:', error)
      uiStore.notifyError('Error', 'No se pudieron cargar las novedades')
    }
  }
}

function goBack() {
  router.push(`/nominas/periodo/${periodo.value}/empleado/${empleadoId.value}`)
}

function openCreateModal() {
  isEditing.value = false
  errors.value = {}
  novedadForm.value = {
    id_Parametro: '',
    tipoNovedad: 'INGRESO',
    descripcion: '',
    montoAplicado: 0,
    cantidadHoras: 0,
    is_Gravable: true,
  }
  formModalOpen.value = true
}

function openEditModal(novedad: Novedad) {
  isEditing.value = true
  editingNovedadId.value = novedad.id_Novedad
  errors.value = {}
  novedadForm.value = {
    id_Parametro: novedad.id_Parametro,
    tipoNovedad: novedad.tipoNovedad,
    descripcion: novedad.descripcion || '',
    montoAplicado: novedad.montoAplicado,
    cantidadHoras: novedad.cantidadHoras || 0,
    is_Gravable: novedad.is_Gravable,
  }
  formModalOpen.value = true
}

function validateForm(): boolean {
  errors.value = {}

  if (!novedadForm.value.id_Parametro) {
    errors.value.id_Parametro = 'Debe seleccionar un parámetro'
  }

  if (novedadForm.value.montoAplicado <= 0) {
    errors.value.montoAplicado = 'El monto debe ser mayor a 0'
  }

  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validateForm()) return

  saving.value = true
  try {
    const novedadData = {
      periodo: periodo.value,
      ...novedadForm.value,
    }

    if (isEditing.value) {
      await api.novedad.update(
        empleadoId.value,
        periodo.value,
        editingNovedadId.value,
        { ...novedadData, id_Novedad: editingNovedadId.value }
      )
      uiStore.notifySuccess('Éxito', 'Novedad actualizada correctamente')
    } else {
      await api.novedad.create(empleadoId.value, novedadData)
      uiStore.notifySuccess('Éxito', 'Novedad creada correctamente. La nómina se recalculará automáticamente.')
    }

    formModalOpen.value = false
    await loadNovedades()
  } catch (error: any) {
    console.error('Error saving novedad:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo guardar la novedad'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    saving.value = false
  }
}

function confirmDelete(novedad: Novedad) {
  novedadToDelete.value = novedad
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!novedadToDelete.value) return

  deleting.value = true
  try {
    await api.novedad.delete(
      empleadoId.value,
      periodo.value,
      novedadToDelete.value.id_Novedad
    )
    uiStore.notifySuccess('Éxito', 'Novedad eliminada correctamente. La nómina se recalculará automáticamente.')
    deleteModalOpen.value = false
    await loadNovedades()
  } catch (error: any) {
    console.error('Error deleting novedad:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo eliminar la novedad'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    deleting.value = false
    novedadToDelete.value = null
  }
}

function formatPeriodo(p: string) {
  const [year, month] = p.split('-')
  const monthNames = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ]
  return `${monthNames[parseInt(month) - 1]} ${year}`
}

function formatCurrency(value: number): string {
  return `$${value.toFixed(2)}`
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div class="flex items-center space-x-4">
        <button
          type="button"
          class="text-gray-400 hover:text-gray-600"
          @click="goBack"
          title="Volver al detalle"
        >
          <ArrowLeftIcon class="h-6 w-6" />
        </button>
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Ingresos y Egresos</h1>
          <p v-if="empleado && persona" class="mt-1 text-sm text-gray-500">
            {{ empleadoNombre }} - {{ formatPeriodo(periodo) }}
          </p>
        </div>
      </div>
      <BaseButton variant="primary" @click="openCreateModal">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nueva Novedad
      </BaseButton>
    </div>

    <!-- Info Banner -->
    <div class="bg-blue-50 border-l-4 border-blue-400 p-4">
      <div class="flex">
        <div class="flex-shrink-0">
          <svg class="h-5 w-5 text-blue-400" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
          </svg>
        </div>
        <div class="ml-3">
          <p class="text-sm text-blue-700">
            <strong>Nota:</strong> Al agregar, editar o eliminar novedades, la nómina se recalculará automáticamente con los nuevos valores.
          </p>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Content -->
    <div v-else class="space-y-6">
      <!-- Ingresos -->
      <div class="card">
        <div class="card-header flex items-center justify-between">
          <h2 class="text-lg font-medium text-gray-900">
            Ingresos ({{ ingresos.length }})
          </h2>
        </div>
        <div class="card-body">
          <DataTable
            :columns="columns"
            :data="ingresos as unknown as Record<string, unknown>[]"
            empty-text="No hay ingresos registrados. Click en 'Nueva Novedad' para agregar."
          >
            <template #cell-tipoNovedad="{ value }">
              <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                Ingreso
              </span>
            </template>
            <template #cell-montoAplicado="{ value }">
              <span class="text-green-600 font-medium">{{ formatCurrency(value as number) }}</span>
            </template>
            <template #cell-is_Gravable="{ value }">
              <span :class="value ? 'text-green-600' : 'text-gray-400'">
                {{ value ? '✓' : '-' }}
              </span>
            </template>
            <template #actions="{ row }">
              <div class="flex items-center space-x-2">
                <button
                  type="button"
                  class="text-gray-400 hover:text-blue-600"
                  title="Editar"
                  @click="openEditModal(row as unknown as Novedad)"
                >
                  <PencilIcon class="h-5 w-5" />
                </button>
                <button
                  type="button"
                  class="text-gray-400 hover:text-red-600"
                  title="Eliminar"
                  @click="confirmDelete(row as unknown as Novedad)"
                >
                  <TrashIcon class="h-5 w-5" />
                </button>
              </div>
            </template>
          </DataTable>
        </div>
      </div>

      <!-- Egresos -->
      <div class="card">
        <div class="card-header flex items-center justify-between">
          <h2 class="text-lg font-medium text-gray-900">
            Egresos ({{ egresos.length }})
          </h2>
        </div>
        <div class="card-body">
          <DataTable
            :columns="columns"
            :data="egresos as unknown as Record<string, unknown>[]"
            empty-text="No hay egresos registrados. Click en 'Nueva Novedad' para agregar."
          >
            <template #cell-tipoNovedad="{ value }">
              <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                Egreso
              </span>
            </template>
            <template #cell-montoAplicado="{ value }">
              <span class="text-red-600 font-medium">{{ formatCurrency(value as number) }}</span>
            </template>
            <template #cell-is_Gravable="{ value }">
              <span class="text-gray-400">-</span>
            </template>
            <template #actions="{ row }">
              <div class="flex items-center space-x-2">
                <button
                  type="button"
                  class="text-gray-400 hover:text-blue-600"
                  title="Editar"
                  @click="openEditModal(row as unknown as Novedad)"
                >
                  <PencilIcon class="h-5 w-5" />
                </button>
                <button
                  type="button"
                  class="text-gray-400 hover:text-red-600"
                  title="Eliminar"
                  @click="confirmDelete(row as unknown as Novedad)"
                >
                  <TrashIcon class="h-5 w-5" />
                </button>
              </div>
            </template>
          </DataTable>
        </div>
      </div>
    </div>

    <!-- Form Modal -->
    <BaseModal
      :open="formModalOpen"
      :title="isEditing ? 'Editar Novedad' : 'Nueva Novedad'"
      @close="formModalOpen = false"
    >
      <div class="space-y-4">
        <FormSelect
          v-model="novedadForm.tipoNovedad"
          label="Tipo de Novedad"
          :options="TIPO_NOVEDAD_OPTIONS"
          required
        />

        <FormSelect
          v-model="novedadForm.id_Parametro"
          label="Concepto"
          :options="parametrosDisponibles"
          required
          :error="errors.id_Parametro"
          help-text="Seleccione el concepto de la novedad"
        />

        <FormInput
          v-model.number="novedadForm.montoAplicado"
          type="number"
          label="Monto"
          step="0.01"
          min="0"
          required
          :error="errors.montoAplicado"
        />

        <FormInput
          v-model.number="novedadForm.cantidadHoras"
          type="number"
          label="Cantidad de Horas (opcional)"
          step="0.5"
          min="0"
          help-text="Para horas extras u otros conceptos por horas"
        />

        <FormTextarea
          v-model="novedadForm.descripcion"
          label="Descripción (opcional)"
          rows="3"
          help-text="Descripción adicional de la novedad"
        />

        <FormCheckbox
          v-if="novedadForm.tipoNovedad === 'INGRESO'"
          v-model="novedadForm.is_Gravable"
          label="Es gravable para IESS e Impuesto a la Renta"
        />
      </div>

      <template #footer>
        <BaseButton variant="outline" @click="formModalOpen = false">
          Cancelar
        </BaseButton>
        <BaseButton variant="primary" :loading="saving" @click="handleSave">
          {{ isEditing ? 'Actualizar' : 'Crear' }} Novedad
        </BaseButton>
      </template>
    </BaseModal>

    <!-- Delete Modal -->
    <BaseModal
      :open="deleteModalOpen"
      title="Eliminar Novedad"
      @close="deleteModalOpen = false"
    >
      <p class="text-sm text-gray-500">
        ¿Está seguro que desea eliminar esta novedad? Esta acción no se puede deshacer y la nómina se recalculará automáticamente.
      </p>
      <div v-if="novedadToDelete" class="mt-4 p-4 bg-gray-50 rounded-lg">
        <dl class="space-y-1">
          <div>
            <dt class="text-xs font-medium text-gray-500">Concepto</dt>
            <dd class="text-sm text-gray-900">{{ novedadToDelete.id_Parametro }}</dd>
          </div>
          <div>
            <dt class="text-xs font-medium text-gray-500">Monto</dt>
            <dd class="text-sm text-gray-900">{{ formatCurrency(novedadToDelete.montoAplicado) }}</dd>
          </div>
        </dl>
      </div>

      <template #footer>
        <BaseButton variant="outline" @click="deleteModalOpen = false">
          Cancelar
        </BaseButton>
        <BaseButton variant="danger" :loading="deleting" @click="handleDelete">
          Eliminar
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>


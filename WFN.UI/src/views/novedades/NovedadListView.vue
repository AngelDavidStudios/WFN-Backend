<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { PlusIcon, PencilIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Novedad, Empleado, Persona, Parametro, DropdownOption } from '@/types'
import { TIPO_NOVEDAD_OPTIONS, getOptionLabel } from '@/constants'
import { formatDateForInput } from '@/utils'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import FormSelect from '@/components/forms/FormSelect.vue'
import FormInput from '@/components/forms/FormInput.vue'
import FormCheckbox from '@/components/forms/FormCheckbox.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const uiStore = useUIStore()

const loading = ref(true)
const novedades = ref<(Novedad & { nombreEmpleado?: string; nombreParametro?: string })[]>([])
const empleados = ref<Map<string, Empleado>>(new Map())
const personas = ref<Map<string, Persona>>(new Map())
const parametros = ref<Map<string, Parametro>>(new Map())

// Filters
const selectedEmpleado = ref('')
const selectedPeriod = ref('')
const empleadosOptions = ref<DropdownOption<string>[]>([])
const parametrosOptions = ref<DropdownOption<string>[]>([])

// Current period
const currentPeriod = computed(() => {
  const now = new Date()
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`
})

// Form modal
const formModalOpen = ref(false)
const isEditing = ref(false)
const saving = ref(false)
const novedadForm = ref({
  id_Parametro: '',
  periodo: '',
  tipoNovedad: 'INGRESO' as 'INGRESO' | 'EGRESO' | 'PROVISION',
  fechaIngresada: '',
  descripcion: '',
  montoAplicado: 0,
  is_Gravable: true,
})
const editingNovedadId = ref('')

// Delete modal
const deleteModalOpen = ref(false)
const novedadToDelete = ref<Novedad | null>(null)
const deleting = ref(false)

const columns = [
  { key: 'nombreEmpleado', label: 'Empleado' },
  { key: 'periodo', label: 'Periodo', width: '100px' },
  { key: 'tipoNovedad', label: 'Tipo', width: '100px' },
  { key: 'nombreParametro', label: 'Parámetro' },
  { key: 'montoAplicado', label: 'Monto', align: 'right' as const },
  { key: 'is_Gravable', label: 'Gravable', width: '80px', align: 'center' as const },
]

async function loadData() {
  loading.value = true
  try {
    const [empleadosData, personasData, parametrosData] = await Promise.all([
      api.empleado.getAll(),
      api.persona.getAll(),
      api.parametro.getAll(),
    ])

    personasData.forEach((p) => personas.value.set(p.id_Persona, p))
    empleadosData.forEach((e) => empleados.value.set(e.id_Empleado, e))
    parametrosData.forEach((p) => parametros.value.set(p.id_Parametro, p))

    empleadosOptions.value = [
      { value: '', label: 'Todos los empleados' },
      ...empleadosData.map((e) => {
        const persona = personas.value.get(e.id_Persona)
        return {
          value: e.id_Empleado,
          label: persona ? `${persona.primerNombre} ${persona.apellidoPaterno}` : e.id_Empleado,
        }
      }),
    ]

    parametrosOptions.value = parametrosData.map((p) => ({
      value: p.id_Parametro,
      label: `${p.nombre} (${p.tipo})`,
    }))

    selectedPeriod.value = currentPeriod.value

    if (empleadosData.length > 0 && empleadosData[0]) {
      selectedEmpleado.value = empleadosData[0].id_Empleado
      await loadNovedades()
    }
  } catch (error) {
    console.error('Error loading data:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar los datos')
  } finally {
    loading.value = false
  }
}

async function loadNovedades() {
  if (!selectedEmpleado.value) {
    novedades.value = []
    return
  }

  try {
    let novedadesData: Novedad[]
    if (selectedPeriod.value) {
      novedadesData = await api.novedad.getByPeriodo(selectedEmpleado.value, selectedPeriod.value)
    } else {
      novedadesData = await api.novedad.getByEmpleado(selectedEmpleado.value)
    }

    const empleado = empleados.value.get(selectedEmpleado.value)
    const persona = empleado ? personas.value.get(empleado.id_Persona) : null

    novedades.value = novedadesData.map((n) => ({
      ...n,
      nombreEmpleado: persona ? `${persona.primerNombre} ${persona.apellidoPaterno}` : 'N/A',
      nombreParametro: parametros.value.get(n.id_Parametro)?.nombre || 'N/A',
    }))
  } catch {
    novedades.value = []
  }
}

function openCreateModal() {
  isEditing.value = false
  novedadForm.value = {
    id_Parametro: '',
    periodo: selectedPeriod.value || currentPeriod.value,
    tipoNovedad: 'INGRESO',
    fechaIngresada: formatDateForInput(new Date()),
    descripcion: '',
    montoAplicado: 0,
    is_Gravable: true,
  }
  formModalOpen.value = true
}

function openEditModal(novedad: Novedad) {
  isEditing.value = true
  editingNovedadId.value = novedad.id_Novedad
  novedadForm.value = {
    id_Parametro: novedad.id_Parametro,
    periodo: novedad.periodo,
    tipoNovedad: novedad.tipoNovedad,
    fechaIngresada: novedad.fechaIngresada?.split('T')[0] || '',
    descripcion: novedad.descripcion,
    montoAplicado: novedad.montoAplicado,
    is_Gravable: novedad.is_Gravable,
  }
  formModalOpen.value = true
}

async function handleSave() {
  if (!selectedEmpleado.value) return

  saving.value = true
  try {
    if (isEditing.value) {
      await api.novedad.update(
        selectedEmpleado.value,
        novedadForm.value.periodo,
        editingNovedadId.value,
        { ...novedadForm.value, id_Novedad: editingNovedadId.value },
      )
      uiStore.notifySuccess('Éxito', 'Novedad actualizada correctamente')
    } else {
      await api.novedad.create(selectedEmpleado.value, novedadForm.value)
      uiStore.notifySuccess('Éxito', 'Novedad creada correctamente')
    }
    formModalOpen.value = false
    await loadNovedades()
  } catch (error) {
    console.error('Error saving novedad:', error)
    uiStore.notifyError('Error', 'No se pudo guardar la novedad')
  } finally {
    saving.value = false
  }
}

function confirmDelete(novedad: Novedad) {
  novedadToDelete.value = novedad
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!novedadToDelete.value || !selectedEmpleado.value) return

  deleting.value = true
  try {
    await api.novedad.delete(
      selectedEmpleado.value,
      novedadToDelete.value.periodo,
      novedadToDelete.value.id_Novedad,
    )
    uiStore.notifySuccess('Éxito', 'Novedad eliminada correctamente')
    deleteModalOpen.value = false
    await loadNovedades()
  } catch (error) {
    console.error('Error deleting novedad:', error)
    uiStore.notifyError('Error', 'No se pudo eliminar la novedad')
  } finally {
    deleting.value = false
    novedadToDelete.value = null
  }
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
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Novedades</h1>
        <p class="mt-1 text-sm text-gray-500">Gestión de novedades de nómina por empleado</p>
      </div>
      <BaseButton variant="primary" :disabled="!selectedEmpleado" @click="openCreateModal">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nueva Novedad
      </BaseButton>
    </div>

    <!-- Filters -->
    <div class="card">
      <div class="card-body">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <FormSelect
            v-model="selectedEmpleado"
            label="Empleado"
            :options="empleadosOptions"
            @update:model-value="loadNovedades"
          />
          <FormInput
            v-model="selectedPeriod"
            type="month"
            label="Periodo"
            @change="loadNovedades"
          />
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Table -->
    <DataTable
      v-else
      :columns="columns"
      :data="novedades as unknown as Record<string, unknown>[]"
      empty-text="No hay novedades para este empleado/periodo"
    >
      <template #cell-tipoNovedad="{ value }">
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
          {{ getOptionLabel(TIPO_NOVEDAD_OPTIONS, value as string) }}
        </span>
      </template>
      <template #cell-montoAplicado="{ value }">
        {{ formatCurrency(value as number) }}
      </template>
      <template #cell-is_Gravable="{ value }">
        <span :class="value ? 'text-green-600' : 'text-gray-400'">
          {{ value ? 'Sí' : 'No' }}
        </span>
      </template>
      <template #actions="{ row }">
        <div class="flex items-center space-x-2">
          <button
            type="button"
            class="text-gray-400 hover:text-primary-600"
            title="Editar"
            @click.stop="openEditModal(row as unknown as Novedad)"
          >
            <PencilIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-red-600"
            title="Eliminar"
            @click.stop="confirmDelete(row as unknown as Novedad)"
          >
            <TrashIcon class="h-5 w-5" />
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Form Modal -->
    <BaseModal
      :open="formModalOpen"
      :title="isEditing ? 'Editar Novedad' : 'Nueva Novedad'"
      size="lg"
      @close="formModalOpen = false"
    >
      <div class="space-y-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <FormSelect
            v-model="novedadForm.tipoNovedad"
            label="Tipo de Novedad"
            :options="TIPO_NOVEDAD_OPTIONS"
            required
          />
          <FormSelect
            v-model="novedadForm.id_Parametro"
            label="Parámetro"
            :options="parametrosOptions"
            placeholder="Seleccione un parámetro"
            required
          />
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <FormInput
            v-model="novedadForm.periodo"
            type="month"
            label="Periodo"
            required
          />
          <FormInput
            v-model="novedadForm.fechaIngresada"
            type="date"
            label="Fecha"
            required
          />
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <FormInput
            v-model="novedadForm.montoAplicado"
            type="number"
            label="Monto ($)"
            required
          />
          <FormCheckbox
            v-model="novedadForm.is_Gravable"
            label="¿Es gravable para IESS?"
          />
        </div>
        <FormInput
          v-model="novedadForm.descripcion"
          label="Descripción"
          placeholder="Descripción opcional"
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
        ¿Está seguro que desea eliminar esta novedad? Esta acción no se puede deshacer.
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

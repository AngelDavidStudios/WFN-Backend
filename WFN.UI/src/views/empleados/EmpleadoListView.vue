<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { PlusIcon, PencilIcon, TrashIcon, EyeIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Empleado, Persona, Departamento } from '@/types'
import { STATUS_LABORAL_OPTIONS, getOptionLabel } from '@/constants'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'

const router = useRouter()
const uiStore = useUIStore()

const loading = ref(true)
const empleados = ref<(Empleado & { nombreCompleto?: string; departamentoNombre?: string })[]>([])
const personas = ref<Map<string, Persona>>(new Map())
const departamentos = ref<Map<string, Departamento>>(new Map())
const deleteModalOpen = ref(false)
const empleadoToDelete = ref<Empleado | null>(null)
const deleting = ref(false)

const columns = [
  { key: 'nombreCompleto', label: 'Empleado' },
  { key: 'departamentoNombre', label: 'Departamento' },
  { key: 'salarioBaseDisplay', label: 'Salario Base', align: 'right' as const },
  { key: 'fechaIngresoDisplay', label: 'Fecha Ingreso', width: '120px' },
  { key: 'statusDisplay', label: 'Estado', width: '120px' },
]

async function loadData() {
  loading.value = true
  try {
    // Load all required data in parallel
    const [empleadosData, personasData, departamentosData] = await Promise.all([
      api.empleado.getAll(),
      api.persona.getAll(),
      api.departamento.getAll(),
    ])

    // Create maps for quick lookup
    personasData.forEach((p) => personas.value.set(p.id_Persona, p))
    departamentosData.forEach((d) => departamentos.value.set(d.id_Departamento, d))

    // Enrich empleados with display data
    empleados.value = empleadosData.map((e) => {
      const persona = personas.value.get(e.id_Persona)
      const depto = departamentos.value.get(e.id_Departamento)
      return {
        ...e,
        nombreCompleto: persona
          ? `${persona.primerNombre} ${persona.apellidoPaterno}`
          : 'N/A',
        departamentoNombre: depto?.nombre || 'N/A',
        salarioBaseDisplay: `$${e.salarioBase.toFixed(2)}`,
        fechaIngresoDisplay: e.fechaIngreso?.split('T')[0] || '-',
        statusDisplay: getOptionLabel(STATUS_LABORAL_OPTIONS, e.statusLaboral),
      }
    })
  } catch (error) {
    console.error('Error loading data:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar los empleados')
  } finally {
    loading.value = false
  }
}

function goToNew() {
  router.push('/empleados/nuevo')
}

function goToEdit(empleado: Empleado) {
  router.push(`/empleados/${empleado.id_Empleado}/editar`)
}

function goToView(empleado: Empleado) {
  router.push(`/empleados/${empleado.id_Empleado}`)
}

function confirmDelete(empleado: Empleado) {
  empleadoToDelete.value = empleado
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!empleadoToDelete.value) return

  deleting.value = true
  try {
    await api.empleado.delete(empleadoToDelete.value.id_Empleado)
    uiStore.notifySuccess('Éxito', 'Empleado eliminado correctamente')
    deleteModalOpen.value = false
    await loadData()
  } catch (error) {
    console.error('Error deleting empleado:', error)
    uiStore.notifyError('Error', 'No se pudo eliminar el empleado')
  } finally {
    deleting.value = false
    empleadoToDelete.value = null
  }
}

function handleRowClick(row: Record<string, unknown>) {
  const empleado = row as unknown as Empleado
  goToView(empleado)
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
        <h1 class="text-2xl font-bold text-gray-900">Empleados</h1>
        <p class="mt-1 text-sm text-gray-500">Gestión de empleados del sistema</p>
      </div>
      <BaseButton variant="primary" @click="goToNew">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nuevo Empleado
      </BaseButton>
    </div>

    <!-- Table -->
    <DataTable
      :columns="columns"
      :data="empleados as unknown as Record<string, unknown>[]"
      :loading="loading"
      empty-text="No hay empleados registrados"
      @row-click="handleRowClick"
    >
      <template #cell-statusDisplay="{ value }">
        <span
          :class="[
            'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
            value === 'Activo'
              ? 'bg-green-100 text-green-800'
              : value === 'Inactivo'
                ? 'bg-red-100 text-red-800'
                : value === 'Con Licencia'
                  ? 'bg-yellow-100 text-yellow-800'
                  : 'bg-gray-100 text-gray-800',
          ]"
        >
          {{ value }}
        </span>
      </template>
      <template #actions="{ row }">
        <div class="flex items-center space-x-2">
          <button
            type="button"
            class="text-gray-400 hover:text-primary-600"
            title="Ver"
            @click.stop="goToView(row as unknown as Empleado)"
          >
            <EyeIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-primary-600"
            title="Editar"
            @click.stop="goToEdit(row as unknown as Empleado)"
          >
            <PencilIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-red-600"
            title="Eliminar"
            @click.stop="confirmDelete(row as unknown as Empleado)"
          >
            <TrashIcon class="h-5 w-5" />
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Delete Confirmation Modal -->
    <BaseModal :open="deleteModalOpen" title="Confirmar Eliminación" @close="deleteModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea eliminar este empleado? Esta acción no se puede deshacer.
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

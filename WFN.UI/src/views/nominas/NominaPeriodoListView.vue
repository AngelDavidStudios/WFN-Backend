<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import {
  ArrowLeftIcon,
  PlusIcon,
  EyeIcon,
  DocumentTextIcon,
  CheckCircleIcon,
  XCircleIcon,
} from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Nomina, Empleado, WorkspaceNomina } from '@/types'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import DataTable from '@/components/tables/DataTable.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const periodo = computed(() => route.params.periodo as string)

const loading = ref(true)
const loadingWorkspace = ref(true)
const workspace = ref<WorkspaceNomina | null>(null)
const nominas = ref<Nomina[]>([])
const empleados = ref<Empleado[]>([])
const personas = ref<any[]>([]) // Agregar personas
const empleadosActivos = computed(() => empleados.value.filter((e) => e.statusLaboral === 0))
const generateModalOpen = ref(false)
const selectedEmpleado = ref<string>('')
const generating = ref(false)

const columns = [
  { key: 'empleadoNombre', label: 'Empleado' },
  { key: 'totalIngresos', label: 'Total Ingresos', align: 'right' as const },
  { key: 'totalEgresos', label: 'Total Egresos', align: 'right' as const },
  { key: 'netoAPagar', label: 'Neto a Pagar', align: 'right' as const },
  { key: 'estadoDisplay', label: 'Estado', align: 'center' as const },
]

const empleadosSinNomina = computed(() => {
  const empleadosConNomina = nominas.value.map((n) => n.id_Empleado)
  return empleadosActivos.value.filter((e) => !empleadosConNomina.includes(e.id_Empleado))
})

async function loadWorkspace() {
  loadingWorkspace.value = true
  try {
    const data = await api.workspace.getByPeriodo(periodo.value)
    workspace.value = data
  } catch (error: any) {
    console.error('Error loading workspace:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo cargar el período'
    uiStore.notifyError('Error', errorMessage)
    router.push('/nominas')
  } finally {
    loadingWorkspace.value = false
  }
}

async function loadNominas() {
  loading.value = true
  try {
    const data = await api.nomina.getByPeriodoGlobal(periodo.value)

    console.log('Nóminas recibidas:', data)
    console.log('Empleados cargados:', empleados.value.length)

    nominas.value = data.map((n: Nomina) => {
      console.log('Procesando nómina con id_Empleado:', n.id_Empleado)
      const empleadoNombre = getEmpleadoNombre(n.id_Empleado)
      console.log('Nombre obtenido:', empleadoNombre)

      return {
        ...n,
        empleadoNombre,
        totalIngresos: `$${n.totalIngresos.toFixed(2)}`,
        totalEgresos: `$${n.totalEgresos.toFixed(2)}`,
        netoAPagar: `$${n.netoAPagar.toFixed(2)}`,
        estadoDisplay: n.isCerrada ? 'Cerrada' : 'Abierta',
      }
    })

    console.log('Nóminas procesadas:', nominas.value)
  } catch (error: any) {
    console.error('Error loading nominas:', error)
    if (error.response?.status !== 404) {
      const errorMessage = error.response?.data?.message || 'No se pudieron cargar las nóminas'
      uiStore.notifyError('Error', errorMessage)
    }
    nominas.value = []
  } finally {
    loading.value = false
  }
}

async function loadEmpleados() {
  try {
    const data = await api.empleado.getAll()
    empleados.value = data // Cargar TODOS para mostrar nombres en nóminas existentes
    console.log('Empleados cargados:', empleados.value.length, empleados.value)
  } catch (error: any) {
    console.error('Error loading empleados:', error)
  }
}

async function loadPersonas() {
  try {
    const data = await api.persona.getAll()
    personas.value = data
    console.log('Personas cargadas:', personas.value.length, personas.value)
  } catch (error: any) {
    console.error('Error loading personas:', error)
  }
}

function goBack() {
  router.push('/nominas')
}

function openGenerateModal() {
  selectedEmpleado.value = ''
  generateModalOpen.value = true
}

async function handleGenerate() {
  if (!selectedEmpleado.value) {
    uiStore.notifyError('Error', 'Debe seleccionar un empleado')
    return
  }

  generating.value = true
  try {
    const nominaGenerada = await api.nomina.generar({
      empleadoId: selectedEmpleado.value,
      periodo: periodo.value,
    })

    uiStore.notifySuccess(
      'Éxito',
      'Nómina generada correctamente. Click en la fila para ver detalles y agregar ingresos/egresos.'
    )
    generateModalOpen.value = false

    // Recargar la lista para mostrar la nueva nómina
    await loadNominas()

  } catch (error: any) {
    console.error('Error generating nomina:', error)

    // Manejo específico de error de duplicado
    if (error.response?.status === 400 && error.response?.data?.message?.includes('Ya existe')) {
      uiStore.notifyError('Nómina Duplicada', error.response.data.message)
      // Cerrar modal y recargar lista para mostrar la nómina existente
      generateModalOpen.value = false
      await loadNominas()
    } else {
      const errorMessage = error.response?.data?.message || 'No se pudo generar la nómina'
      uiStore.notifyError('Error', errorMessage)
    }
  } finally {
    generating.value = false
  }
}

function goToDetail(row: Record<string, unknown>) {
  const nomina = row as unknown as Nomina
  router.push(`/nominas/periodo/${periodo.value}/empleado/${nomina.id_Empleado}`)
}

function formatPeriodo(p: string) {
  const [year, month] = p.split('-')
  const monthNames = [
    'Enero',
    'Febrero',
    'Marzo',
    'Abril',
    'Mayo',
    'Junio',
    'Julio',
    'Agosto',
    'Septiembre',
    'Octubre',
    'Noviembre',
    'Diciembre',
  ]
  return `${monthNames[parseInt(month) - 1]} ${year}`
}

function getEmpleadoNombre(empleadoId: string): string {
  if (!empleadoId) {
    console.warn('EmpleadoId is empty or undefined')
    return 'Empleado Desconocido'
  }

  // 1. Buscar el empleado por ID
  const empleado = empleados.value.find((e) => e.id_Empleado === empleadoId)

  if (!empleado) {
    console.warn(`Empleado no encontrado con ID: ${empleadoId}`)
    console.log('Empleados disponibles:', empleados.value.map(e => e.id_Empleado))
    return empleadoId // Mostrar el ID si no se encuentra
  }

  // 2. Buscar la persona asociada al empleado
  const persona = personas.value.find((p) => p.id_Persona === empleado.id_Persona)

  if (!persona) {
    console.warn(`Persona no encontrada con ID: ${empleado.id_Persona} para empleado ${empleadoId}`)
    console.log('Personas disponibles:', personas.value.map(p => p.id_Persona))
    return `Empleado ${empleadoId} (Persona no encontrada)`
  }

  // 3. Construir nombre completo de la persona
  const nombreCompleto = [
    persona.primerNombre,
    persona.segundoNombre,
    persona.apellidoPaterno,
    persona.apellidoMaterno
  ]
    .filter(Boolean) // Eliminar valores vacíos/null/undefined
    .join(' ')

  return nombreCompleto || empleadoId
}

onMounted(async () => {
  await Promise.all([loadWorkspace(), loadEmpleados(), loadPersonas()])
  await loadNominas()
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
          title="Volver"
        >
          <ArrowLeftIcon class="h-6 w-6" />
        </button>
        <div>
          <h1 class="text-2xl font-bold text-gray-900">
            Nóminas - {{ formatPeriodo(periodo) }}
          </h1>
          <p v-if="workspace" class="mt-1 text-sm text-gray-500">
            Período {{ periodo }} -
            <span :class="workspace.estado === 0 ? 'text-green-600' : 'text-gray-600'">
              {{ workspace.estado === 0 ? 'Abierto' : 'Cerrado' }}
            </span>
          </p>
        </div>
      </div>
      <BaseButton
        v-if="workspace?.estado === 0 && empleadosSinNomina.length > 0"
        variant="primary"
        @click="openGenerateModal"
      >
        <PlusIcon class="h-5 w-5 mr-2" />
        Generar Nómina
      </BaseButton>
    </div>

    <!-- Info Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="card">
        <div class="card-body">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <DocumentTextIcon class="h-10 w-10 text-blue-600" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-500">Nóminas Generadas</p>
              <p class="text-2xl font-semibold text-gray-900">{{ nominas.length }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-body">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <CheckCircleIcon class="h-10 w-10 text-green-600" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-500">Empleados Activos</p>
              <p class="text-2xl font-semibold text-gray-900">{{ empleadosActivos.length }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-body">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <XCircleIcon class="h-10 w-10 text-orange-600" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-500">Pendientes</p>
              <p class="text-2xl font-semibold text-gray-900">{{ empleadosSinNomina.length }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Mensaje informativo -->
    <div v-if="nominas.length > 0" class="bg-blue-50 border-l-4 border-blue-400 p-4">
      <div class="flex">
        <div class="flex-shrink-0">
          <svg class="h-5 w-5 text-blue-400" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
          </svg>
        </div>
        <div class="ml-3">
          <p class="text-sm text-blue-700">
            <strong>Tip:</strong> Haga click en cualquier fila de la tabla para ver el detalle de la nómina y agregar ingresos/egresos adicionales.
          </p>
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
      :data="nominas as unknown as Record<string, unknown>[]"
      :loading="loading"
      empty-text="No hay nóminas generadas para este período"
      @row-click="goToDetail"
    >
      <template #actions="{ row }">
        <button
          type="button"
          class="text-gray-400 hover:text-blue-600"
          title="Ver detalle"
          @click.stop="goToDetail(row)"
        >
          <EyeIcon class="h-5 w-5" />
        </button>
      </template>
    </DataTable>

    <!-- Generate Modal -->
    <BaseModal
      :open="generateModalOpen"
      title="Generar Nómina"
      @close="generateModalOpen = false"
    >
      <div class="space-y-4">
        <p class="text-sm text-gray-500">
          Seleccione el empleado para el cual desea generar la nómina del período
          {{ formatPeriodo(periodo) }}
        </p>

        <div>
          <label class="label">Empleado</label>
          <select v-model="selectedEmpleado" class="input">
            <option value="">Seleccione un empleado</option>
            <option
              v-for="empleado in empleadosSinNomina"
              :key="empleado.id_Empleado"
              :value="empleado.id_Empleado"
            >
              {{ empleado.primerNombre }} {{ empleado.segundoNombre ? empleado.segundoNombre + ' ' : '' }}{{ empleado.apellidoPaterno }} {{ empleado.apellidoMaterno }} ({{ empleado.id_Empleado }})
            </option>
          </select>
        </div>

        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <p class="text-sm text-blue-800">
            <strong>Nota:</strong> La nómina se calculará automáticamente según el salario del
            empleado, sus novedades del período y los parámetros configurados.
          </p>
        </div>
      </div>

      <template #footer>
        <BaseButton variant="outline" @click="generateModalOpen = false" :disabled="generating">
          Cancelar
        </BaseButton>
        <BaseButton variant="primary" :loading="generating" :disabled="generating" @click="handleGenerate">
          Generar Nómina
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>


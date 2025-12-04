<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { CurrencyDollarIcon, PlayIcon, ArrowPathIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Nomina, Empleado, Persona, WorkspaceNomina, DropdownOption } from '@/types'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import FormSelect from '@/components/forms/FormSelect.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const uiStore = useUIStore()

const loading = ref(true)
const generating = ref(false)
const nominas = ref<(Nomina & { nombreEmpleado?: string })[]>([])
const empleados = ref<Map<string, Empleado>>(new Map())
const personas = ref<Map<string, Persona>>(new Map())
const workspaces = ref<WorkspaceNomina[]>([])

// Current period
const currentPeriod = computed(() => {
  const now = new Date()
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`
})

const selectedPeriod = ref(currentPeriod.value)
const periodoOptions = ref<DropdownOption<string>[]>([])

// Generate modal
const generateModalOpen = ref(false)
const selectedEmpleado = ref('')
const empleadosOptions = ref<DropdownOption<string>[]>([])

// Resumen
const resumen = computed(() => {
  const totalIngresos = nominas.value.reduce((sum, n) => sum + n.totalIngresos, 0)
  const totalEgresos = nominas.value.reduce((sum, n) => sum + n.totalEgresos, 0)
  const totalNeto = nominas.value.reduce((sum, n) => sum + n.netoAPagar, 0)
  return { totalIngresos, totalEgresos, totalNeto, count: nominas.value.length }
})

const columns = [
  { key: 'nombreEmpleado', label: 'Empleado' },
  { key: 'totalIngresosGravados', label: 'Ingresos Gravados', align: 'right' as const },
  { key: 'totalIngresosNoGravados', label: 'Ingresos No Gravados', align: 'right' as const },
  { key: 'totalIngresos', label: 'Total Ingresos', align: 'right' as const },
  { key: 'totalEgresos', label: 'Total Egresos', align: 'right' as const },
  { key: 'netoAPagar', label: 'Neto a Pagar', align: 'right' as const },
]

async function loadData() {
  loading.value = true
  try {
    // Load all required data
    const [empleadosData, personasData, workspacesData] = await Promise.all([
      api.empleado.getAll(),
      api.persona.getAll(),
      api.workspace.getAll(),
    ])

    // Create maps
    personasData.forEach((p) => personas.value.set(p.id_Persona, p))
    empleadosData.forEach((e) => empleados.value.set(e.id_Empleado, e))

    // Build options
    empleadosOptions.value = empleadosData.map((e) => {
      const persona = personas.value.get(e.id_Persona)
      return {
        value: e.id_Empleado,
        label: persona ? `${persona.primerNombre} ${persona.apellidoPaterno}` : e.id_Empleado,
      }
    })

    // Build periodo options
    workspaces.value = workspacesData
    periodoOptions.value = workspacesData.map((w) => ({
      value: w.periodo,
      label: `${w.periodo} (${w.estado === 0 ? 'Abierto' : 'Cerrado'})`,
    }))

    // Add current period if not exists
    if (!periodoOptions.value.find((p) => p.value === currentPeriod.value)) {
      periodoOptions.value.unshift({
        value: currentPeriod.value,
        label: `${currentPeriod.value} (Nuevo)`,
      })
    }

    await loadNominas()
  } catch (error) {
    console.error('Error loading data:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar los datos')
  } finally {
    loading.value = false
  }
}

async function loadNominas() {
  try {
    const nominasData = await api.nomina.getByPeriodoGlobal(selectedPeriod.value)
    nominas.value = nominasData.map((n) => {
      const empleado = empleados.value.get(n.id_Empleado)
      const persona = empleado ? personas.value.get(empleado.id_Persona) : null
      return {
        ...n,
        nombreEmpleado: persona
          ? `${persona.primerNombre} ${persona.apellidoPaterno}`
          : n.id_Empleado,
      }
    })
  } catch {
    nominas.value = []
  }
}

async function handlePeriodChange() {
  await loadNominas()
}

function openGenerateModal() {
  selectedEmpleado.value = ''
  generateModalOpen.value = true
}

async function generateNomina() {
  if (!selectedEmpleado.value) {
    uiStore.notifyWarning('Atención', 'Debe seleccionar un empleado')
    return
  }

  generating.value = true
  try {
    await api.nomina.generar({
      empleadoId: selectedEmpleado.value,
      periodo: selectedPeriod.value,
    })
    uiStore.notifySuccess('Éxito', 'Nómina generada correctamente')
    generateModalOpen.value = false
    await loadNominas()
  } catch (error) {
    console.error('Error generating nomina:', error)
    uiStore.notifyError('Error', 'No se pudo generar la nómina')
  } finally {
    generating.value = false
  }
}

async function generateAllNominas() {
  generating.value = true
  try {
    const activeEmpleados = Array.from(empleados.value.values()).filter(
      (e) => e.statusLaboral === 0, // Active
    )

    for (const empleado of activeEmpleados) {
      try {
        await api.nomina.generar({
          empleadoId: empleado.id_Empleado,
          periodo: selectedPeriod.value,
        })
      } catch {
        console.warn(`Error generating nomina for ${empleado.id_Empleado}`)
      }
    }

    uiStore.notifySuccess('Éxito', `Nóminas generadas para ${activeEmpleados.length} empleados`)
    await loadNominas()
  } catch (error) {
    console.error('Error generating all nominas:', error)
    uiStore.notifyError('Error', 'Error al generar las nóminas')
  } finally {
    generating.value = false
  }
}

async function recalcularNomina(nomina: Nomina) {
  try {
    await api.nomina.recalcular({
      empleadoId: nomina.id_Empleado,
      periodo: nomina.periodo,
    })
    uiStore.notifySuccess('Éxito', 'Nómina recalculada correctamente')
    await loadNominas()
  } catch (error) {
    console.error('Error recalculating nomina:', error)
    uiStore.notifyError('Error', 'No se pudo recalcular la nómina')
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
        <h1 class="text-2xl font-bold text-gray-900">Nóminas</h1>
        <p class="mt-1 text-sm text-gray-500">Gestión y cálculo de nóminas por periodo</p>
      </div>
      <div class="flex space-x-3">
        <BaseButton variant="secondary" @click="openGenerateModal">
          <CurrencyDollarIcon class="h-5 w-5 mr-2" />
          Generar Individual
        </BaseButton>
        <BaseButton variant="primary" :loading="generating" @click="generateAllNominas">
          <PlayIcon class="h-5 w-5 mr-2" />
          Generar Todas
        </BaseButton>
      </div>
    </div>

    <!-- Period Selector -->
    <div class="card">
      <div class="card-body">
        <div class="flex items-center space-x-4">
          <div class="w-64">
            <FormSelect
              v-model="selectedPeriod"
              label="Periodo"
              :options="periodoOptions"
              @update:model-value="handlePeriodChange"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <template v-else>
      <!-- Summary Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div class="card">
          <div class="card-body">
            <p class="text-sm text-gray-500">Total Empleados</p>
            <p class="text-2xl font-bold text-gray-900">{{ resumen.count }}</p>
          </div>
        </div>
        <div class="card">
          <div class="card-body">
            <p class="text-sm text-gray-500">Total Ingresos</p>
            <p class="text-2xl font-bold text-green-600">{{ formatCurrency(resumen.totalIngresos) }}</p>
          </div>
        </div>
        <div class="card">
          <div class="card-body">
            <p class="text-sm text-gray-500">Total Egresos</p>
            <p class="text-2xl font-bold text-red-600">{{ formatCurrency(resumen.totalEgresos) }}</p>
          </div>
        </div>
        <div class="card">
          <div class="card-body">
            <p class="text-sm text-gray-500">Total Neto a Pagar</p>
            <p class="text-2xl font-bold text-primary-600">{{ formatCurrency(resumen.totalNeto) }}</p>
          </div>
        </div>
      </div>

      <!-- Table -->
      <DataTable
        :columns="columns"
        :data="nominas as unknown as Record<string, unknown>[]"
        empty-text="No hay nóminas para este periodo"
      >
        <template #cell-totalIngresosGravados="{ value }">
          {{ formatCurrency(value as number) }}
        </template>
        <template #cell-totalIngresosNoGravados="{ value }">
          {{ formatCurrency(value as number) }}
        </template>
        <template #cell-totalIngresos="{ value }">
          <span class="font-medium text-green-600">{{ formatCurrency(value as number) }}</span>
        </template>
        <template #cell-totalEgresos="{ value }">
          <span class="font-medium text-red-600">{{ formatCurrency(value as number) }}</span>
        </template>
        <template #cell-netoAPagar="{ value }">
          <span class="font-bold text-primary-600">{{ formatCurrency(value as number) }}</span>
        </template>
        <template #actions="{ row }">
          <button
            type="button"
            class="text-gray-400 hover:text-primary-600"
            title="Recalcular"
            @click.stop="recalcularNomina(row as unknown as Nomina)"
          >
            <ArrowPathIcon class="h-5 w-5" />
          </button>
        </template>
      </DataTable>
    </template>

    <!-- Generate Modal -->
    <BaseModal :open="generateModalOpen" title="Generar Nómina Individual" @close="generateModalOpen = false">
      <div class="space-y-4">
        <p class="text-gray-500">
          Seleccione el empleado para generar su nómina del periodo {{ selectedPeriod }}
        </p>
        <FormSelect
          v-model="selectedEmpleado"
          label="Empleado"
          :options="empleadosOptions"
          placeholder="Seleccione un empleado"
        />
      </div>
      <template #footer>
        <BaseButton variant="outline" @click="generateModalOpen = false"> Cancelar </BaseButton>
        <BaseButton variant="primary" :loading="generating" @click="generateNomina">
          Generar Nómina
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

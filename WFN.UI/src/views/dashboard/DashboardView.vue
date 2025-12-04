<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import {
  UsersIcon,
  BuildingOfficeIcon,
  CurrencyDollarIcon,
  CalendarIcon,
} from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore, useAuthStore } from '@/stores'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const uiStore = useUIStore()
const authStore = useAuthStore()

const loading = ref(true)
const stats = ref({
  totalEmpleados: 0,
  totalDepartamentos: 0,
  totalNominasDelMes: 0,
  periodoActual: '',
})

const greeting = computed(() => {
  const hour = new Date().getHours()
  if (hour < 12) return 'Buenos días'
  if (hour < 18) return 'Buenas tardes'
  return 'Buenas noches'
})

const currentPeriod = computed(() => {
  const now = new Date()
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`
})

async function loadStats() {
  loading.value = true
  try {
    // Load employees
    const empleados = await api.empleado.getAll()
    stats.value.totalEmpleados = empleados.length

    // Load departments
    const departamentos = await api.departamento.getAll()
    stats.value.totalDepartamentos = departamentos.length

    // Load current period payrolls
    try {
      const nominas = await api.nomina.getByPeriodoGlobal(currentPeriod.value)
      stats.value.totalNominasDelMes = nominas.length
    } catch {
      stats.value.totalNominasDelMes = 0
    }

    stats.value.periodoActual = currentPeriod.value
  } catch (error) {
    console.error('Error loading dashboard stats:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar las estadísticas')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadStats()
})

const statCards = computed(() => [
  {
    title: 'Total Empleados',
    value: stats.value.totalEmpleados,
    icon: UsersIcon,
    color: 'bg-blue-500',
    bgColor: 'bg-blue-100',
    textColor: 'text-blue-600',
  },
  {
    title: 'Departamentos',
    value: stats.value.totalDepartamentos,
    icon: BuildingOfficeIcon,
    color: 'bg-green-500',
    bgColor: 'bg-green-100',
    textColor: 'text-green-600',
  },
  {
    title: 'Nóminas del Periodo',
    value: stats.value.totalNominasDelMes,
    icon: CurrencyDollarIcon,
    color: 'bg-purple-500',
    bgColor: 'bg-purple-100',
    textColor: 'text-purple-600',
  },
  {
    title: 'Periodo Actual',
    value: stats.value.periodoActual,
    icon: CalendarIcon,
    color: 'bg-orange-500',
    bgColor: 'bg-orange-100',
    textColor: 'text-orange-600',
  },
])
</script>

<template>
  <div class="space-y-6">
    <!-- Welcome Header -->
    <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <h1 class="text-2xl font-bold text-gray-900">
        {{ greeting }}, {{ authStore.userProfile?.first_name || 'Usuario' }}
      </h1>
      <p class="mt-1 text-gray-500">Bienvenido al Sistema de Gestión de Nóminas WFN</p>
    </div>

    <!-- Stats Cards -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div
        v-for="(stat, index) in statCards"
        :key="index"
        class="card"
      >
        <div class="card-body">
          <div class="flex items-center">
            <div :class="['flex-shrink-0 p-3 rounded-lg', stat.bgColor]">
              <component :is="stat.icon" :class="['h-6 w-6', stat.textColor]" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-500">{{ stat.title }}</p>
              <p class="text-2xl font-semibold text-gray-900">{{ stat.value }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="card">
      <div class="card-header">
        <h2 class="text-lg font-semibold text-gray-900">Acciones Rápidas</h2>
      </div>
      <div class="card-body">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <RouterLink
            to="/empleados/nuevo"
            class="flex items-center p-4 rounded-lg border border-gray-200 hover:bg-gray-50 transition-colors"
          >
            <UsersIcon class="h-8 w-8 text-primary-600" />
            <div class="ml-3">
              <p class="font-medium text-gray-900">Nuevo Empleado</p>
              <p class="text-sm text-gray-500">Registrar un nuevo empleado</p>
            </div>
          </RouterLink>

          <RouterLink
            to="/nominas"
            class="flex items-center p-4 rounded-lg border border-gray-200 hover:bg-gray-50 transition-colors"
          >
            <CurrencyDollarIcon class="h-8 w-8 text-green-600" />
            <div class="ml-3">
              <p class="font-medium text-gray-900">Generar Nómina</p>
              <p class="text-sm text-gray-500">Calcular nóminas del periodo</p>
            </div>
          </RouterLink>

          <RouterLink
            to="/novedades"
            class="flex items-center p-4 rounded-lg border border-gray-200 hover:bg-gray-50 transition-colors"
          >
            <CalendarIcon class="h-8 w-8 text-purple-600" />
            <div class="ml-3">
              <p class="font-medium text-gray-900">Registrar Novedad</p>
              <p class="text-sm text-gray-500">Agregar ingresos o descuentos</p>
            </div>
          </RouterLink>
        </div>
      </div>
    </div>

    <!-- Recent Activity Placeholder -->
    <div class="card">
      <div class="card-header">
        <h2 class="text-lg font-semibold text-gray-900">Actividad Reciente</h2>
      </div>
      <div class="card-body">
        <p class="text-gray-500 text-center py-8">
          Las estadísticas y actividad reciente aparecerán aquí
        </p>
      </div>
    </div>
  </div>
</template>

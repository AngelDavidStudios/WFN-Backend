<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import {
  UsersIcon,
  BuildingOfficeIcon,
  CurrencyDollarIcon,
  BanknotesIcon,
  ArrowTrendingUpIcon,
} from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore, useAuthStore } from '@/stores'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import LineChart from '@/components/charts/LineChart.vue'
import BarChart from '@/components/charts/BarChart.vue'
import DonutChart from '@/components/charts/DonutChart.vue'

const uiStore = useUIStore()
const authStore = useAuthStore()

const loading = ref(true)
const stats = ref({
  totalEmpleados: 0,
  totalDepartamentos: 0,
  totalNominasDelMes: 0,
  periodoActual: '',
  totalNomina: 0,
  promedioSalario: 0,
})

// Datos para gráficos
const empleadosPorDepartamento = ref<{ departamento: string; cantidad: number }[]>([])
const nominasPorMes = ref<{ mes: string; total: number; ingresos: number; egresos: number }[]>([])

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

// Obtener últimos 6 meses
const getLastSixMonths = () => {
  const months = []
  const now = new Date()
  for (let i = 5; i >= 0; i--) {
    const date = new Date(now.getFullYear(), now.getMonth() - i, 1)
    const periodo = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`
    const monthName = date.toLocaleDateString('es-ES', { month: 'short', year: 'numeric' })
    months.push({ periodo, name: monthName })
  }
  return months
}

async function loadStats() {
  loading.value = true
  try {
    // Load employees
    const empleados = await api.empleado.getAll()
    stats.value.totalEmpleados = empleados.length

    // Load departments
    const departamentos = await api.departamento.getAll()
    stats.value.totalDepartamentos = departamentos.length

    // Calcular empleados por departamento
    const empleadosPorDept: { [key: string]: number } = {}

    console.log('📊 Calculando empleados por departamento...')
    console.log('Total empleados:', empleados.length)
    console.log('Total departamentos:', departamentos.length)

    // Debug: Ver estructura completa de un empleado
    if (empleados.length > 0) {
      console.log('🔍 Ejemplo de empleado completo:', JSON.stringify(empleados[0], null, 2))
    }

    // Debug: Ver estructura completa de un departamento
    if (departamentos.length > 0) {
      console.log('🔍 Ejemplo de departamento completo:', JSON.stringify(departamentos[0], null, 2))
    }

    for (const emp of empleados) {
      // El campo correcto es id_Departamento según el tipo Empleado
      const deptId = emp.id_Departamento

      console.log(`🔍 Empleado ${emp.id_Empleado || 'sin ID'}:`, {
        id_Departamento: deptId,
        tieneId: !!deptId,
        valorCompleto: deptId
      })

      if (deptId) {
        // Buscar por id_Departamento (no por "id")
        const dept = departamentos.find(d => d.id_Departamento === deptId)
        const deptName = dept?.nombre || 'Sin Departamento'
        empleadosPorDept[deptName] = (empleadosPorDept[deptName] || 0) + 1

        if (dept) {
          console.log(`✅ Empleado asignado a: ${deptName}`)
        } else {
          console.log(`⚠️ Departamento no encontrado para ID: ${deptId}`)
        }
      } else {
        // Contar empleados sin departamento
        empleadosPorDept['Sin Departamento'] = (empleadosPorDept['Sin Departamento'] || 0) + 1
        console.log('❌ Empleado sin departamento')
      }
    }

    empleadosPorDepartamento.value = Object.entries(empleadosPorDept).map(([departamento, cantidad]) => ({
      departamento,
      cantidad
    }))

    console.log('📊 Resultado empleados por departamento:', empleadosPorDepartamento.value)

    // Load nóminas de los últimos 6 meses
    const lastSixMonths = getLastSixMonths()
    const nominasPorPeriodo: any[] = []

    for (const { periodo, name } of lastSixMonths) {
      try {
        const nominas = await api.nomina.getByPeriodoGlobal(periodo)
        const totalIngresos = nominas.reduce((sum, n) => sum + (n.totalIngresos || 0), 0)
        const totalEgresos = nominas.reduce((sum, n) => sum + (n.totalEgresos || 0), 0)
        const totalNeto = nominas.reduce((sum, n) => sum + (n.netoAPagar || 0), 0)

        nominasPorPeriodo.push({
          mes: name,
          total: totalNeto,
          ingresos: totalIngresos,
          egresos: totalEgresos,
        })
      } catch {
        nominasPorPeriodo.push({
          mes: name,
          total: 0,
          ingresos: 0,
          egresos: 0,
        })
      }
    }
    nominasPorMes.value = nominasPorPeriodo

    // Estadísticas del mes actual
    try {
      const nominasActuales = await api.nomina.getByPeriodoGlobal(currentPeriod.value)
      stats.value.totalNominasDelMes = nominasActuales.length
      stats.value.totalNomina = nominasActuales.reduce((sum, n) => sum + (n.netoAPagar || 0), 0)
      stats.value.promedioSalario = nominasActuales.length > 0
        ? stats.value.totalNomina / nominasActuales.length
        : 0
    } catch {
      stats.value.totalNominasDelMes = 0
      stats.value.totalNomina = 0
      stats.value.promedioSalario = 0
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
    bgColor: 'bg-blue-100',
    textColor: 'text-blue-600',
  },
  {
    title: 'Departamentos',
    value: stats.value.totalDepartamentos,
    icon: BuildingOfficeIcon,
    bgColor: 'bg-green-100',
    textColor: 'text-green-600',
  },
  {
    title: 'Nóminas Procesadas',
    value: stats.value.totalNominasDelMes,
    icon: CurrencyDollarIcon,
    bgColor: 'bg-purple-100',
    textColor: 'text-purple-600',
  },
  {
    title: 'Total Nómina Mes',
    value: `$${stats.value.totalNomina.toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`,
    icon: BanknotesIcon,
    bgColor: 'bg-orange-100',
    textColor: 'text-orange-600',
  },
])

// Datos para gráficos computados
const departmentChartData = computed(() => ({
  labels: empleadosPorDepartamento.value.map(d => d.departamento),
  data: empleadosPorDepartamento.value.map(d => d.cantidad),
}))

const payrollTrendData = computed(() => ({
  labels: nominasPorMes.value.map(n => n.mes),
  data: nominasPorMes.value.map(n => n.total),
}))

const payrollComparisonData = computed(() => ({
  labels: nominasPorMes.value.map(n => n.mes),
  datasets: [
    {
      label: 'Ingresos',
      data: nominasPorMes.value.map(n => n.ingresos),
      backgroundColor: 'rgba(34, 197, 94, 0.8)',
      borderColor: 'rgb(34, 197, 94)',
    },
    {
      label: 'Egresos',
      data: nominasPorMes.value.map(n => n.egresos),
      backgroundColor: 'rgba(239, 68, 68, 0.8)',
      borderColor: 'rgb(239, 68, 68)',
    },
  ],
}))
</script>

<template>
  <div class="space-y-6">
    <!-- Welcome Header - MANTENIDO COMO SOLICITASTE -->
    <div class="bg-gradient-to-r from-indigo-600 to-blue-700 rounded-lg shadow-lg p-6 text-white">
      <h1 class="text-3xl font-bold text-white">
        {{ greeting }}, {{ authStore.userProfile?.first_name || 'Usuario' }}
      </h1>
      <p class="mt-2 text-white opacity-90">Sistema de Gestión de Nóminas WFN - {{ stats.periodoActual }}</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <template v-else>
      <!-- KPI Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div
          v-for="(stat, index) in statCards"
          :key="index"
          class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow"
        >
          <div class="flex items-center justify-between">
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-500 mb-1">{{ stat.title }}</p>
              <p class="text-2xl font-bold text-gray-900">{{ stat.value }}</p>
            </div>
            <div :class="['flex-shrink-0 p-3 rounded-lg', stat.bgColor]">
              <component :is="stat.icon" :class="['h-8 w-8', stat.textColor]" />
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Row 1 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Payroll Trend Chart -->
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-semibold text-gray-900">Tendencia de Nóminas</h2>
            <ArrowTrendingUpIcon class="h-5 w-5 text-primary-600" />
          </div>
          <p class="text-sm text-gray-500 mb-4">Total de nómina de los últimos 6 meses</p>
          <div class="h-80">
            <LineChart
              v-if="payrollTrendData.data.length > 0"
              :labels="payrollTrendData.labels"
              :data="payrollTrendData.data"
              label="Total Nómina"
              border-color="#6366f1"
              background-color="rgba(99, 102, 241, 0.1)"
            />
            <div v-else class="flex items-center justify-center h-full text-gray-400">
              Sin datos disponibles
            </div>
          </div>
        </div>

        <!-- Employees by Department Chart -->
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-semibold text-gray-900">Empleados por Departamento</h2>
            <BuildingOfficeIcon class="h-5 w-5 text-green-600" />
          </div>
          <p class="text-sm text-gray-500 mb-4">Distribución actual de empleados</p>
          <div class="h-80">
            <DonutChart
              v-if="departmentChartData.labels.length > 0"
              :labels="departmentChartData.labels"
              :data="departmentChartData.data"
              :colors="['#6366f1', '#8b5cf6', '#ec4899', '#f59e0b', '#10b981', '#3b82f6']"
            />
            <div v-else class="flex items-center justify-center h-full text-gray-400">
              Sin datos disponibles
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Row 2: Income vs Expenses -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-gray-900">Ingresos vs Egresos</h2>
          <CurrencyDollarIcon class="h-5 w-5 text-purple-600" />
        </div>
        <p class="text-sm text-gray-500 mb-4">Comparación mensual de ingresos y descuentos</p>
        <div class="h-80">
          <BarChart
            v-if="payrollComparisonData.datasets[0].data.length > 0"
            :labels="payrollComparisonData.labels"
            :datasets="payrollComparisonData.datasets"
          />
          <div v-else class="flex items-center justify-center h-full text-gray-400">
            Sin datos disponibles
          </div>
        </div>
      </div>

      <!-- Summary Cards with Gradient -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg shadow-lg p-6 text-white">
          <div class="flex items-center justify-between mb-2">
            <h3 class="text-sm font-medium text-blue-100">Promedio Salarial</h3>
            <BanknotesIcon class="h-6 w-6 text-blue-200" />
          </div>
          <p class="text-3xl font-bold">
            ${{ stats.promedioSalario.toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }}
          </p>
          <p class="text-sm text-blue-100 mt-2">Promedio del periodo actual</p>
        </div>

        <div class="bg-gradient-to-br from-green-500 to-green-600 rounded-lg shadow-lg p-6 text-white">
          <div class="flex items-center justify-between mb-2">
            <h3 class="text-sm font-medium text-green-100">Tasa de Procesamiento</h3>
            <UsersIcon class="h-6 w-6 text-green-200" />
          </div>
          <p class="text-3xl font-bold">
            {{ stats.totalEmpleados > 0 ? ((stats.totalNominasDelMes / stats.totalEmpleados) * 100).toFixed(1) : 0 }}%
          </p>
          <p class="text-sm text-green-100 mt-2">{{ stats.totalNominasDelMes }} de {{ stats.totalEmpleados }} empleados</p>
        </div>

        <div class="bg-gradient-to-br from-purple-500 to-purple-600 rounded-lg shadow-lg p-6 text-white">
          <div class="flex items-center justify-between mb-2">
            <h3 class="text-sm font-medium text-purple-100">Total Departamentos</h3>
            <BuildingOfficeIcon class="h-6 w-6 text-purple-200" />
          </div>
          <p class="text-3xl font-bold">{{ stats.totalDepartamentos }}</p>
          <p class="text-sm text-purple-100 mt-2">Áreas activas en la organización</p>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200">
        <div class="p-6 border-b border-gray-200">
          <h2 class="text-lg font-semibold text-gray-900">Acciones Rápidas</h2>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <RouterLink
              to="/empleados/nuevo"
              class="flex items-center p-4 rounded-lg border-2 border-gray-200 hover:border-primary-500 hover:bg-primary-50 transition-all group"
            >
              <div class="flex-shrink-0 p-3 rounded-lg bg-blue-100 group-hover:bg-blue-200 transition-colors">
                <UsersIcon class="h-6 w-6 text-blue-600" />
              </div>
              <div class="ml-4">
                <p class="font-medium text-gray-900 group-hover:text-primary-700">Nuevo Empleado</p>
                <p class="text-sm text-gray-500">Registrar un nuevo empleado</p>
              </div>
            </RouterLink>

            <RouterLink
              to="/nominas"
              class="flex items-center p-4 rounded-lg border-2 border-gray-200 hover:border-green-500 hover:bg-green-50 transition-all group"
            >
              <div class="flex-shrink-0 p-3 rounded-lg bg-green-100 group-hover:bg-green-200 transition-colors">
                <CurrencyDollarIcon class="h-6 w-6 text-green-600" />
              </div>
              <div class="ml-4">
                <p class="font-medium text-gray-900 group-hover:text-green-700">Gestionar Nóminas</p>
                <p class="text-sm text-gray-500">Ver y procesar nóminas</p>
              </div>
            </RouterLink>

            <RouterLink
              to="/departamentos"
              class="flex items-center p-4 rounded-lg border-2 border-gray-200 hover:border-purple-500 hover:bg-purple-50 transition-all group"
            >
              <div class="flex-shrink-0 p-3 rounded-lg bg-purple-100 group-hover:bg-purple-200 transition-colors">
                <BuildingOfficeIcon class="h-6 w-6 text-purple-600" />
              </div>
              <div class="ml-4">
                <p class="font-medium text-gray-900 group-hover:text-purple-700">Departamentos</p>
                <p class="text-sm text-gray-500">Administrar áreas</p>
              </div>
            </RouterLink>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>


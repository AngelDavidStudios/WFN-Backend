<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ArrowLeftIcon, PlusIcon, ChartBarIcon, ArrowPathIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Nomina, Empleado, Novedad, Provision } from '@/types'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const periodo = computed(() => route.params.periodo as string)
const empleadoId = computed(() => route.params.empleadoId as string)

const loading = ref(true)
const nomina = ref<Nomina | null>(null)
const empleado = ref<Empleado | null>(null)
const novedades = ref<Novedad[]>([])
const provisiones = ref<Provision[]>([])

const ingresos = computed(() => novedades.value.filter((n) => n.tipoNovedad === 'INGRESO'))
const egresos = computed(() => novedades.value.filter((n) => n.tipoNovedad === 'EGRESO'))

// Ingresos automáticos calculados por el sistema (mensualizaciones)
const ingresosAutomaticos = computed(() => {
  if (!empleado.value || !nomina.value) return []

  const automaticos: Array<{
    concepto: string
    descripcion: string
    monto: number
    gravable: boolean
    tipo: string
  }> = []

  // Salario Base siempre está
  automaticos.push({
    concepto: 'SALARIO_BASE',
    descripcion: 'Salario Base',
    monto: empleado.value.salarioBase,
    gravable: true,
    tipo: 'BASE'
  })

  // Total gravado para calcular mensualizaciones
  const totalGravado = nomina.value.totalIngresosGravados

  // Décimo Tercero Mensualizado
  if (empleado.value.is_DecimoTercMensual) {
    automaticos.push({
      concepto: 'DECIMO_TERCERO_MENSUAL',
      descripcion: 'Décimo Tercero Mensualizado',
      monto: totalGravado / 12,
      gravable: false,
      tipo: 'MENSUALIZADO'
    })
  }

  // Décimo Cuarto Mensualizado
  if (empleado.value.is_DecimoCuartoMensual) {
    const sbu = 470 // SBU Ecuador 2025
    automaticos.push({
      concepto: 'DECIMO_CUARTO_MENSUAL',
      descripcion: 'Décimo Cuarto Mensualizado',
      monto: sbu / 12,
      gravable: false,
      tipo: 'MENSUALIZADO'
    })
  }

  // Fondos de Reserva Mensualizados
  if (empleado.value.is_FondoReservaMensual) {
    automaticos.push({
      concepto: 'FONDOS_RESERVA_MENSUAL',
      descripcion: 'Fondos de Reserva Mensualizado',
      monto: totalGravado * 0.0833, // 8.33%
      gravable: false,
      tipo: 'MENSUALIZADO'
    })
  }

  return automaticos
})

// Total de ingresos automáticos
const totalIngresosAutomaticos = computed(() => {
  return ingresosAutomaticos.value.reduce((sum, i) => sum + i.monto, 0)
})

// Total de ingresos manuales (novedades)
const totalIngresosManuales = computed(() => {
  return ingresos.value.reduce((sum, i) => sum + i.montoAplicado, 0)
})

async function loadData(retry = 0) {
  loading.value = true
  try {
    // Cargar empleado primero (siempre existe)
    const empleadoData = await api.empleado.getById(empleadoId.value)
    empleado.value = empleadoData

    // Cargar nómina (puede no existir aún si acabamos de redirigir)
    try {
      const nominaData = await api.nomina.getByPeriodo(empleadoId.value, periodo.value)
      nomina.value = nominaData
    } catch (nominaError: any) {
      if (nominaError.response?.status === 404 && retry < 2) {
        // Reintentar después de un pequeño delay (la nómina puede estar guardándose)
        await new Promise(resolve => setTimeout(resolve, 1000))
        return loadData(retry + 1)
      }
      throw nominaError
    }

    // Cargar novedades del período
    try {
      const novedadesData = await api.novedad.getByPeriodo(empleadoId.value, periodo.value)
      novedades.value = novedadesData
    } catch (novedadError: any) {
      // Si no hay novedades, está bien, dejamos el array vacío
      if (novedadError.response?.status === 404) {
        novedades.value = []
      } else {
        throw novedadError
      }
    }

    // Cargar provisiones del período
    try {
      const provisionesData = await api.provision.getByPeriodo(empleadoId.value, periodo.value)
      provisiones.value = provisionesData
    } catch (provisionError: any) {
      // Si no hay provisiones, está bien, dejamos el array vacío
      if (provisionError.response?.status === 404) {
        provisiones.value = []
      } else {
        console.warn('Error loading provisiones:', provisionError)
        provisiones.value = []
      }
    }
  } catch (error: any) {
    console.error('Error loading data:', error)
    const errorMessage = error.response?.data?.message || 'No se pudieron cargar los datos de la nómina'
    uiStore.notifyError('Error', errorMessage)
    router.push(`/nominas/periodo/${periodo.value}`)
  } finally {
    loading.value = false
  }
}

function goBack() {
  router.push(`/nominas/periodo/${periodo.value}`)
}

function goToNovedades() {
  router.push(`/nominas/periodo/${periodo.value}/empleado/${empleadoId.value}/novedades`)
}

function formatPeriodo(p: string) {
  const [year, month] = p.split('-')
  const monthNames = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre',
  ]
  return `${monthNames[parseInt(month) - 1]} ${year}`
}

function formatTipoProvision(tipo: string): string {
  const tipos: Record<string, string> = {
    DECIMO_TERCERO: 'Décimo Tercero',
    DECIMO_CUARTO: 'Décimo Cuarto',
    FONDO_RESERVA: 'Fondo de Reserva',
    VACACIONES: 'Vacaciones',
    IESS_PATRONAL: 'IESS Patronal',
  }
  return tipos[tipo] || tipo
}

function getProvisionColor(tipo: string): string {
  const colors: Record<string, string> = {
    DECIMO_TERCERO: 'text-blue-600',
    DECIMO_CUARTO: 'text-green-600',
    FONDO_RESERVA: 'text-purple-600',
    VACACIONES: 'text-orange-600',
    IESS_PATRONAL: 'text-indigo-600',
  }
  return colors[tipo] || 'text-gray-600'
}

const totalProvisiones = computed(() => {
  return provisiones.value.reduce((sum, p) => sum + p.valorMensual, 0)
})

const totalAcumuladoProvisiones = computed(() => {
  return provisiones.value.reduce((sum, p) => sum + p.acumulado, 0)
})

// Función para recargar datos manualmente
async function reloadData() {
  await loadData()
  uiStore.notifySuccess('Actualizado', 'Los datos de la nómina han sido actualizados')
}

// Cargar datos inicialmente
onMounted(() => {
  console.log('NominaDetailView mounted - loading data')
  loadData()

  // Verificar si hay una actualización pendiente de novedades
  checkForUpdates()
})

// Verificar si hay cambios desde la vista de novedades
async function checkForUpdates() {
  const updateFlag = localStorage.getItem(`nomina-updated-${empleadoId.value}-${periodo.value}`)
  if (updateFlag) {
    console.log('Detected nomina update flag - waiting for backend recalculation...')
    localStorage.removeItem(`nomina-updated-${empleadoId.value}-${periodo.value}`)
    // Esperar un poco más para asegurar que el backend terminó el recálculo
    await new Promise(resolve => setTimeout(resolve, 800))
    console.log('Reloading nomina data after update...')
    await loadData()
    console.log('Nomina data reloaded. New totals:', {
      ingresos: nomina.value?.totalIngresos,
      egresos: nomina.value?.totalEgresos,
      iess: nomina.value?.iess_AportePersonal,
      neto: nomina.value?.netoAPagar
    })
  }
}

// Observar cambios en fullPath para detectar cuando regresamos de /novedades
let lastPath = route.fullPath
watch(
  () => route.fullPath,
  async (newPath) => {
    // Detectar si estamos en la vista de detalle (no en /novedades)
    const isDetailView = newPath.match(/\/nominas\/periodo\/[^/]+\/empleado\/[^/]+$/)
    const wasInNovedades = lastPath.includes('/novedades')

    if (isDetailView && wasInNovedades) {
      console.log('Returned from novedades view - checking for updates')
      await new Promise(resolve => setTimeout(resolve, 300))
      checkForUpdates()
    }

    lastPath = newPath
  }
)

// También observar cambios en localStorage (para refrescos en la misma pestaña)
watch(
  () => localStorage.getItem(`nomina-updated-${empleadoId.value}-${periodo.value}`),
  (newValue) => {
    if (newValue) {
      console.log('LocalStorage update detected - reloading')
      checkForUpdates()
    }
  }
)
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
          <div class="flex items-center space-x-3">
            <h1 class="text-2xl font-bold text-gray-900">Detalle de Nómina</h1>
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
              Auto-recálculo activo
            </span>
          </div>
          <p v-if="empleado" class="mt-1 text-sm text-gray-500">
            {{ empleado.primerNombre }} {{ empleado.apellidoPaterno }} - {{ formatPeriodo(periodo) }}
          </p>
        </div>
      </div>
      <div class="flex items-center space-x-3">
        <BaseButton variant="outline" @click="reloadData" :loading="loading">
          <ArrowPathIcon class="h-5 w-5 mr-2" />
          Actualizar
        </BaseButton>
        <BaseButton variant="primary" @click="goToNovedades">
          <PlusIcon class="h-5 w-5 mr-2" />
          Agregar Ingresos/Egresos
        </BaseButton>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Content -->
    <div v-else-if="nomina" class="space-y-6">
      <!-- Banner informativo si no hay novedades -->
      <div
        v-if="ingresos.length === 0 && egresos.length === 0"
        class="bg-blue-50 border-l-4 border-blue-400 p-4"
      >
        <div class="flex">
          <div class="flex-shrink-0">
            <svg
              class="h-5 w-5 text-blue-400"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fill-rule="evenodd"
                d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <div class="ml-3">
            <h3 class="text-sm font-medium text-blue-800">Nómina creada - Agregue ingresos y egresos</h3>
            <div class="mt-2 text-sm text-blue-700">
              <p>
                La nómina ha sido creada con el salario base. Para completarla:
              </p>
              <ul class="list-disc list-inside mt-1 space-y-1">
                <li>Click en "Agregar Ingresos/Egresos" para agregar conceptos adicionales</li>
                <li>Agregue ingresos como: horas extras, comisiones, bonos, transporte</li>
                <li>Agregue egresos como: préstamos, anticipos, descuentos, multas</li>
                <li><strong>✨ Auto-recálculo:</strong> La nómina se recalculará automáticamente al guardar cada novedad</li>
                <li>Use el botón "Actualizar" si necesita ver los cambios más recientes</li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <!-- Resumen -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="card">
          <div class="card-body">
            <p class="text-sm font-medium text-gray-500">Total Ingresos</p>
            <p class="text-2xl font-semibold text-green-600">${{ nomina.totalIngresos.toFixed(2) }}</p>
          </div>
        </div>
        <div class="card">
          <div class="card-body">
            <p class="text-sm font-medium text-gray-500">Total Egresos</p>
            <p class="text-2xl font-semibold text-red-600">${{ nomina.totalEgresos.toFixed(2) }}</p>
          </div>
        </div>
        <div class="card">
          <div class="card-body">
            <p class="text-sm font-medium text-gray-500">IESS Personal</p>
            <p class="text-2xl font-semibold text-gray-900">${{ nomina.iess_AportePersonal.toFixed(2) }}</p>
          </div>
        </div>
        <div class="card bg-blue-50">
          <div class="card-body">
            <p class="text-sm font-medium text-blue-700">Neto a Pagar</p>
            <p class="text-2xl font-semibold text-blue-900">${{ nomina.netoAPagar.toFixed(2) }}</p>
          </div>
        </div>
      </div>

      <!-- Ingresos -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Ingresos</h2>
        </div>
        <div class="card-body">
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Concepto
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Descripción
                  </th>
                  <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Monto
                  </th>
                  <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">
                    Gravable
                  </th>
                  <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">
                    Tipo
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <!-- Ingresos Automáticos -->
                <tr
                  v-for="ingreso in ingresosAutomaticos"
                  :key="ingreso.concepto"
                  class="bg-blue-50"
                >
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {{ ingreso.concepto }}
                  </td>
                  <td class="px-6 py-4 text-sm text-gray-700">
                    {{ ingreso.descripcion }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-right text-green-600 font-medium">
                    ${{ ingreso.monto.toFixed(2) }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-center">
                    <span v-if="ingreso.gravable" class="text-green-600">✓</span>
                    <span v-else class="text-gray-400">-</span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                      {{ ingreso.tipo === 'BASE' ? 'Automático' : 'Mensualizado' }}
                    </span>
                  </td>
                </tr>

                <!-- Ingresos Manuales (Novedades) -->
                <tr v-if="ingresos.length === 0 && ingresosAutomaticos.length > 0">
                  <td colspan="5" class="px-6 py-4 text-center text-sm text-gray-500 bg-gray-50">
                    No hay ingresos adicionales registrados.
                    <span class="block text-xs text-gray-400 mt-1">
                      Click en "Agregar Ingresos/Egresos" para agregar conceptos como horas extras, comisiones, bonos, etc.
                    </span>
                  </td>
                </tr>
                <tr
                  v-for="ingreso in ingresos"
                  :key="ingreso.id_Novedad"
                >
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {{ ingreso.id_Parametro }}
                  </td>
                  <td class="px-6 py-4 text-sm text-gray-500">
                    {{ ingreso.descripcion || '-' }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-right text-green-600 font-medium">
                    ${{ ingreso.montoAplicado.toFixed(2) }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-center">
                    <span v-if="ingreso.is_Gravable" class="text-green-600">✓</span>
                    <span v-else class="text-gray-400">-</span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                      Manual
                    </span>
                  </td>
                </tr>

                <!-- Totales -->
                <tr v-if="ingresos.length > 0 || ingresosAutomaticos.length > 0" class="bg-gray-100 font-semibold">
                  <td colspan="2" class="px-6 py-4 text-sm text-gray-900">
                    Total Ingresos
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-right text-green-700 font-bold">
                    ${{ nomina.totalIngresos.toFixed(2) }}
                  </td>
                  <td colspan="2"></td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Info sobre ingresos automáticos -->
          <div class="mt-4 bg-blue-50 border-l-4 border-blue-400 p-4">
            <div class="flex">
              <div class="flex-shrink-0">
                <svg class="h-5 w-5 text-blue-400" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
                </svg>
              </div>
              <div class="ml-3">
                <h3 class="text-sm font-medium text-blue-800">Ingresos Automáticos vs Manuales</h3>
                <div class="mt-2 text-sm text-blue-700">
                  <ul class="list-disc list-inside space-y-1">
                    <li><strong>Automáticos/Mensualizados (azul):</strong> Calculados por el sistema según la configuración del empleado. No se pueden editar manualmente.</li>
                    <li><strong>Manuales (verde):</strong> Agregados por el usuario como novedades (horas extras, comisiones, bonos, etc.)</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Egresos -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Egresos ({{ egresos.length }})</h2>
        </div>
        <div class="card-body">
          <div v-if="egresos.length === 0" class="text-center py-8">
            <p class="text-gray-500 mb-2">No hay egresos registrados</p>
            <p class="text-sm text-gray-400">
              Click en "Agregar Ingresos/Egresos" para agregar descuentos como préstamos, anticipos,
              multas, etc.
            </p>
          </div>
          <div v-else class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Concepto
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Descripción
                  </th>
                  <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Monto
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="egreso in egresos" :key="egreso.id_Novedad">
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {{ egreso.id_Parametro }}
                  </td>
                  <td class="px-6 py-4 text-sm text-gray-500">
                    {{ egreso.descripcion || '-' }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-right text-red-600 font-medium">
                    ${{ egreso.montoAplicado.toFixed(2) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Provisiones -->
      <div class="card border-2 border-purple-100">
        <div class="card-header bg-purple-50">
          <div class="flex items-center justify-between">
            <div class="flex items-center space-x-2">
              <ChartBarIcon class="h-5 w-5 text-purple-600" />
              <h2 class="text-lg font-medium text-gray-900">
                Provisiones ({{ provisiones.length }})
              </h2>
            </div>
            <div class="text-right">
              <p class="text-xs text-gray-500">Total Mensual</p>
              <p class="text-lg font-semibold text-purple-700">
                ${{ totalProvisiones.toFixed(2) }}
              </p>
            </div>
          </div>
        </div>
        <div class="card-body">
          <!-- Explicación -->
          <div class="mb-6 bg-purple-50 border-l-4 border-purple-400 p-4">
            <div class="flex">
              <div class="flex-shrink-0">
                <svg
                  class="h-5 w-5 text-purple-400"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fill-rule="evenodd"
                    d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                    clip-rule="evenodd"
                  />
                </svg>
              </div>
              <div class="ml-3">
                <h3 class="text-sm font-medium text-purple-800">Sobre las Provisiones</h3>
                <div class="mt-2 text-sm text-purple-700">
                  <p>
                    Las provisiones son cálculos automáticos que el sistema realiza mensualmente
                    según la configuración del empleado:
                  </p>
                  <ul class="list-disc list-inside mt-2 space-y-1">
                    <li><strong>Mensualizadas:</strong> Se pagan cada mes junto con el salario</li>
                    <li>
                      <strong>Acumuladas:</strong> Se van sumando mes a mes hasta completar el
                      periodo (ej: Diciembre a Noviembre para Décimo Tercero)
                    </li>
                    <li>
                      El "Acumulado" muestra cuánto se ha provisionado hasta el momento para este
                      concepto
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          </div>

          <!-- Tabla de Provisiones -->
          <div v-if="provisiones.length === 0" class="text-center py-8">
            <ChartBarIcon class="mx-auto h-12 w-12 text-gray-400" />
            <p class="mt-2 text-gray-500">No hay provisiones calculadas para este período</p>
            <p class="text-sm text-gray-400">
              Las provisiones se calculan automáticamente al generar la nómina
            </p>
          </div>
          <div v-else class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Tipo de Provisión
                  </th>
                  <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Valor Mensual
                  </th>
                  <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Acumulado
                  </th>
                  <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">
                    Estado
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="provision in provisiones" :key="provision.id_Provision" class="hover:bg-gray-50">
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="flex items-center">
                      <div>
                        <div class="text-sm font-medium text-gray-900">
                          {{ formatTipoProvision(provision.tipoProvision) }}
                        </div>
                        <div class="text-xs text-gray-500">
                          Periodo: {{ formatPeriodo(provision.periodo) }}
                        </div>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-right">
                    <div :class="['text-sm font-semibold', getProvisionColor(provision.tipoProvision)]">
                      ${{ provision.valorMensual.toFixed(2) }}
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-right">
                    <div class="text-sm font-medium text-gray-900">
                      ${{ provision.acumulado.toFixed(2) }}
                    </div>
                    <div class="text-xs text-gray-500">
                      Total: ${{ provision.total.toFixed(2) }}
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <span
                      v-if="provision.isTransferred"
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800"
                    >
                      Transferido
                    </span>
                    <span
                      v-else
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800"
                    >
                      Acumulando
                    </span>
                  </td>
                </tr>
              </tbody>
              <tfoot class="bg-gray-50">
                <tr>
                  <td class="px-6 py-4 text-sm font-semibold text-gray-900">
                    Totales
                  </td>
                  <td class="px-6 py-4 text-right text-sm font-bold text-purple-700">
                    ${{ totalProvisiones.toFixed(2) }}
                  </td>
                  <td class="px-6 py-4 text-right text-sm font-bold text-gray-900">
                    ${{ totalAcumuladoProvisiones.toFixed(2) }}
                  </td>
                  <td></td>
                </tr>
              </tfoot>
            </table>
          </div>
        </div>
      </div>

      <!-- Info adicional -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información Adicional</h2>
        </div>
        <div class="card-body">
          <dl class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
            <div>
              <dt class="text-sm font-medium text-gray-500">Ingresos Gravados</dt>
              <dd class="mt-1 text-sm text-gray-900">${{ nomina.totalIngresosGravados.toFixed(2) }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Ingresos No Gravados</dt>
              <dd class="mt-1 text-sm text-gray-900">${{ nomina.totalIngresosNoGravados.toFixed(2) }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Impuesto a la Renta</dt>
              <dd class="mt-1 text-sm text-gray-900">${{ nomina.ir_Retenido.toFixed(2) }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Fecha de Cálculo</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ nomina.fechaCalculo || '-' }}</dd>
            </div>
          </dl>
        </div>
      </div>
    </div>
  </div>
</template>


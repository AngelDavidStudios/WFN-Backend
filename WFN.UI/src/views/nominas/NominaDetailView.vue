<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ArrowLeftIcon, PlusIcon, PencilIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Nomina, Empleado, Novedad } from '@/types'
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

const ingresos = computed(() => novedades.value.filter((n) => n.tipoNovedad === 'INGRESO'))
const egresos = computed(() => novedades.value.filter((n) => n.tipoNovedad === 'EGRESO'))

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
          title="Volver"
        >
          <ArrowLeftIcon class="h-6 w-6" />
        </button>
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Detalle de Nómina</h1>
          <p v-if="empleado" class="mt-1 text-sm text-gray-500">
            {{ empleado.primerNombre }} {{ empleado.apellidoPaterno }} - {{ formatPeriodo(periodo) }}
          </p>
        </div>
      </div>
      <BaseButton variant="primary" @click="goToNovedades">
        <PlusIcon class="h-5 w-5 mr-2" />
        Agregar Ingresos/Egresos
      </BaseButton>
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
                <li>La nómina se recalculará automáticamente al guardar cada novedad</li>
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
          <h2 class="text-lg font-medium text-gray-900">Ingresos ({{ ingresos.length }})</h2>
        </div>
        <div class="card-body">
          <div v-if="ingresos.length === 0" class="text-center py-8">
            <p class="text-gray-500 mb-2">No hay ingresos registrados</p>
            <p class="text-sm text-gray-400">
              Click en "Agregar Ingresos/Egresos" para agregar conceptos como horas extras,
              comisiones, bonos, etc.
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
                  <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">
                    Gravable
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="ingreso in ingresos" :key="ingreso.id_Novedad">
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
                </tr>
              </tbody>
            </table>
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


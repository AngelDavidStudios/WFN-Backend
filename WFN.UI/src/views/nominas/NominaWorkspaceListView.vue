<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { PlusIcon, CalendarIcon, CheckCircleIcon, XCircleIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { WorkspaceNomina } from '@/types'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import FormInput from '@/components/forms/FormInput.vue'

const router = useRouter()
const uiStore = useUIStore()

const loading = ref(true)
const workspaces = ref<WorkspaceNomina[]>([])
const createModalOpen = ref(false)
const creating = ref(false)
const nuevoPeriodo = ref('')
const errors = ref<Record<string, string>>({})

async function loadWorkspaces() {
  loading.value = true
  try {
    const data = await api.workspace.getAll()
    // Ordenar por periodo descendente (más recientes primero)
    workspaces.value = data.sort((a, b) => b.periodo.localeCompare(a.periodo))
  } catch (error: any) {
    console.error('Error loading workspaces:', error)
    const errorMessage = error.response?.data?.message || 'No se pudieron cargar los períodos'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  nuevoPeriodo.value = ''
  errors.value = {}
  createModalOpen.value = true
}

function validatePeriodo(): boolean {
  errors.value = {}

  // Formato debe ser YYYY-MM
  const regex = /^\d{4}-(0[1-9]|1[0-2])$/
  if (!nuevoPeriodo.value) {
    errors.value.periodo = 'El período es requerido'
    return false
  }
  if (!regex.test(nuevoPeriodo.value)) {
    errors.value.periodo = 'Formato inválido. Use YYYY-MM (ej: 2025-11)'
    return false
  }

  return true
}

async function handleCreate() {
  if (!validatePeriodo()) return

  creating.value = true
  try {
    await api.workspace.create(nuevoPeriodo.value)
    uiStore.notifySuccess('Éxito', `Período ${nuevoPeriodo.value} creado correctamente`)
    createModalOpen.value = false
    await loadWorkspaces()
  } catch (error: any) {
    console.error('Error creating workspace:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo crear el período'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    creating.value = false
  }
}

function goToNominas(workspace: WorkspaceNomina) {
  router.push(`/nominas/periodo/${workspace.periodo}`)
}

function getEstadoText(estado: number) {
  return estado === 0 ? 'Abierto' : 'Cerrado'
}

function getEstadoIcon(estado: number) {
  return estado === 0 ? CheckCircleIcon : XCircleIcon
}

function formatPeriodo(periodo: string) {
  const [year, month] = periodo.split('-')
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

onMounted(() => {
  loadWorkspaces()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Períodos de Nómina</h1>
        <p class="mt-1 text-sm text-gray-500">
          Gestione los períodos mensuales para el cálculo de nóminas
        </p>
      </div>
      <BaseButton variant="primary" @click="openCreateModal">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nuevo Período
      </BaseButton>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Empty State -->
    <div
      v-else-if="workspaces.length === 0"
      class="text-center py-12 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300"
    >
      <CalendarIcon class="mx-auto h-12 w-12 text-gray-400" />
      <h3 class="mt-2 text-sm font-medium text-gray-900">No hay períodos creados</h3>
      <p class="mt-1 text-sm text-gray-500">Comience creando un nuevo período de nómina.</p>
      <div class="mt-6">
        <BaseButton variant="primary" @click="openCreateModal">
          <PlusIcon class="h-5 w-5 mr-2" />
          Crear Primer Período
        </BaseButton>
      </div>
    </div>

    <!-- Workspaces Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="workspace in workspaces"
        :key="workspace.id_Workspace"
        class="card hover:shadow-lg transition-shadow cursor-pointer"
        @click="goToNominas(workspace)"
      >
        <div class="card-body">
          <div class="flex items-start justify-between">
            <div class="flex items-center space-x-3">
              <div class="flex-shrink-0">
                <CalendarIcon class="h-10 w-10 text-blue-600" />
              </div>
              <div>
                <h3 class="text-lg font-medium text-gray-900">
                  {{ formatPeriodo(workspace.periodo) }}
                </h3>
                <p class="text-sm text-gray-500">{{ workspace.periodo }}</p>
              </div>
            </div>
            <span
              :class="[
                'inline-flex items-center px-3 py-1 rounded-full text-xs font-medium',
                workspace.estado === 0 ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800',
              ]"
            >
              <component :is="getEstadoIcon(workspace.estado)" class="h-4 w-4 mr-1" />
              {{ getEstadoText(workspace.estado) }}
            </span>
          </div>

          <div class="mt-4 space-y-2">
            <div class="flex items-center text-sm text-gray-500">
              <span class="font-medium">Creado:</span>
              <span class="ml-2">{{ workspace.fechaCreacion || '-' }}</span>
            </div>
            <div v-if="workspace.fechaCierre" class="flex items-center text-sm text-gray-500">
              <span class="font-medium">Cerrado:</span>
              <span class="ml-2">{{ workspace.fechaCierre }}</span>
            </div>
          </div>

          <div class="mt-4 pt-4 border-t border-gray-200">
            <div class="flex items-center justify-between text-sm">
              <span class="text-gray-500">Click para ver nóminas</span>
              <svg
                class="h-5 w-5 text-gray-400"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9 5l7 7-7 7"
                />
              </svg>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Modal -->
    <BaseModal :open="createModalOpen" title="Crear Nuevo Período" @close="createModalOpen = false">
      <div class="space-y-4">
        <p class="text-sm text-gray-500">
          Ingrese el período en formato YYYY-MM (ejemplo: 2025-11 para Noviembre 2025)
        </p>

        <FormInput
          v-model="nuevoPeriodo"
          label="Período (YYYY-MM)"
          placeholder="2025-11"
          required
          :error="errors.periodo"
          help-text="Formato: Año-Mes (ej: 2025-11)"
        />

        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <p class="text-sm text-blue-800">
            <strong>Nota:</strong> Una vez creado el período, podrá generar nóminas para cada
            empleado activo. El período se crea en estado "Abierto" y puede cerrarse cuando todas
            las nóminas estén procesadas.
          </p>
        </div>
      </div>

      <template #footer>
        <BaseButton variant="outline" @click="createModalOpen = false"> Cancelar </BaseButton>
        <BaseButton variant="primary" :loading="creating" @click="handleCreate">
          Crear Período
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>



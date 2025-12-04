<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { PencilIcon, ArrowLeftIcon, TrashIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Banking, Persona } from '@/types'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import BaseModal from '@/components/common/BaseModal.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const personaId = computed(() => route.params.personaId as string)
const bankId = computed(() => route.params.bankId as string)

const loading = ref(true)
const loadingPersona = ref(true)
const cuenta = ref<Banking | null>(null)
const persona = ref<Persona | null>(null)
const deleteModalOpen = ref(false)
const deleting = ref(false)

const nombrePersona = computed(() => {
  if (!persona.value) return ''
  return [persona.value.primerNombre, persona.value.segundoNombre, persona.value.apellidoPaterno]
    .filter(Boolean)
    .join(' ')
})

const accountTypeDisplay = computed(() => {
  if (!cuenta.value) return ''
  return cuenta.value.accountType === 'AHORROS' ? 'Cuenta de Ahorros' : 'Cuenta Corriente'
})

const dateCreatedDisplay = computed(() => {
  if (!cuenta.value?.dateCreated) return '-'
  const date = new Date(cuenta.value.dateCreated)
  return date.toLocaleDateString('es-EC', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
})

const maskedAccountNumber = computed(() => {
  if (!cuenta.value?.accountNumber) return ''
  const num = cuenta.value.accountNumber
  if (num.length <= 4) return num
  return '*'.repeat(num.length - 4) + num.slice(-4)
})

async function loadPersona() {
  loadingPersona.value = true
  try {
    const data = await api.persona.getById(personaId.value)
    persona.value = data
  } catch (error: any) {
    console.error('Error loading persona:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo cargar la información de la persona'
    uiStore.notifyError('Error', errorMessage)
    router.push('/personas')
  } finally {
    loadingPersona.value = false
  }
}

async function loadCuenta() {
  loading.value = true
  try {
    const data = await api.banking.getById(personaId.value, bankId.value)
    cuenta.value = data
  } catch (error: any) {
    console.error('Error loading cuenta:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo cargar la cuenta bancaria'
    uiStore.notifyError('Error', errorMessage)
    router.push(`/banking/persona/${personaId.value}`)
  } finally {
    loading.value = false
  }
}

function goBack() {
  router.push(`/banking/persona/${personaId.value}`)
}

function goToEdit() {
  router.push(`/banking/persona/${personaId.value}/${bankId.value}/editar`)
}

function confirmDelete() {
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!cuenta.value) return

  deleting.value = true
  try {
    await api.banking.delete(personaId.value, cuenta.value.id_Banking)
    uiStore.notifySuccess('Éxito', 'Cuenta bancaria eliminada correctamente')
    deleteModalOpen.value = false
    router.push(`/banking/persona/${personaId.value}`)
  } catch (error: any) {
    console.error('Error deleting cuenta:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo eliminar la cuenta bancaria'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    deleting.value = false
  }
}

onMounted(async () => {
  await loadPersona()
  await loadCuenta()
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
          <h1 class="text-2xl font-bold text-gray-900">Detalle de Cuenta Bancaria</h1>
          <p v-if="loadingPersona" class="mt-1 text-sm text-gray-500">Cargando...</p>
          <p v-else class="mt-1 text-sm text-gray-500">
            Cuenta de <strong>{{ nombrePersona }}</strong>
          </p>
        </div>
      </div>
      <div v-if="!loading" class="flex space-x-3">
        <BaseButton variant="outline" @click="goToEdit">
          <PencilIcon class="h-5 w-5 mr-2" />
          Editar
        </BaseButton>
        <BaseButton variant="danger" @click="confirmDelete">
          <TrashIcon class="h-5 w-5 mr-2" />
          Eliminar
        </BaseButton>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Content -->
    <div v-else-if="cuenta" class="space-y-6">
      <!-- Información de la Cuenta -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información de la Cuenta</h2>
        </div>
        <div class="card-body">
          <dl class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
            <div>
              <dt class="text-sm font-medium text-gray-500">Banco</dt>
              <dd class="mt-1 text-sm text-gray-900 font-medium">{{ cuenta.bankName }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Tipo de Cuenta</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ accountTypeDisplay }}</dd>
            </div>
            <div class="md:col-span-2">
              <dt class="text-sm font-medium text-gray-500">Número de Cuenta</dt>
              <dd class="mt-1 text-lg text-gray-900 font-mono font-semibold">
                {{ maskedAccountNumber }}
              </dd>
              <dd class="mt-1 text-xs text-gray-500">
                Número completo: {{ cuenta.accountNumber }}
              </dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Código SWIFT</dt>
              <dd class="mt-1 text-sm text-gray-900 font-mono">
                {{ cuenta.swiftCode || 'No especificado' }}
              </dd>
            </div>
          </dl>
        </div>
      </div>

      <!-- Ubicación -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Ubicación</h2>
        </div>
        <div class="card-body">
          <dl class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
            <div>
              <dt class="text-sm font-medium text-gray-500">País</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ cuenta.pais }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Sucursal</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ cuenta.sucursal || 'No especificada' }}</dd>
            </div>
          </dl>
        </div>
      </div>

      <!-- Información del Sistema -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información del Sistema</h2>
        </div>
        <div class="card-body">
          <dl class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
            <div>
              <dt class="text-sm font-medium text-gray-500">ID de Cuenta</dt>
              <dd class="mt-1 text-sm text-gray-900 font-mono">{{ cuenta.id_Banking }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Fecha de Creación</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ dateCreatedDisplay }}</dd>
            </div>
          </dl>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <BaseModal :open="deleteModalOpen" title="Confirmar Eliminación" @close="deleteModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea eliminar la cuenta bancaria de
        <strong>{{ cuenta?.bankName }}</strong> terminada en
        <strong>{{ cuenta?.accountNumber?.slice(-4) }}</strong>? Esta acción no se puede deshacer.
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


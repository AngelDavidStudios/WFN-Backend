<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { PencilIcon, ArrowLeftIcon, TrashIcon, BanknotesIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Persona } from '@/types'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import BaseModal from '@/components/common/BaseModal.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const loading = ref(true)
const persona = ref<Persona | null>(null)
const deleteModalOpen = ref(false)
const deleting = ref(false)

const nombreCompleto = computed(() => {
  if (!persona.value) return ''
  return [
    persona.value.primerNombre,
    persona.value.segundoNombre,
    persona.value.apellidoPaterno,
    persona.value.apellidoMaterno,
  ]
    .filter(Boolean)
    .join(' ')
})

const genderDisplay = computed(() => {
  if (!persona.value) return ''
  switch (persona.value.gender) {
    case 'M':
      return 'Masculino'
    case 'F':
      return 'Femenino'
    default:
      return 'Otro'
  }
})

const fechaNacimientoDisplay = computed(() => {
  if (!persona.value?.dateBirthday) return '-'
  const date = new Date(persona.value.dateBirthday)
  return date.toLocaleDateString('es-EC', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
})

const direccionCompleta = computed(() => {
  if (!persona.value?.direccion) return '-'
  const { calle, numero, piso } = persona.value.direccion
  const parts = [calle, numero, piso].filter(Boolean)
  return parts.length > 0 ? parts.join(', ') : '-'
})

async function loadPersona() {
  loading.value = true
  try {
    const data = await api.persona.getById(route.params.id as string)
    persona.value = data
  } catch (error: any) {
    console.error('Error loading persona:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo cargar la persona'
    uiStore.notifyError('Error', errorMessage)
    router.push('/personas')
  } finally {
    loading.value = false
  }
}

function goBack() {
  router.push('/personas')
}

function goToEdit() {
  router.push(`/personas/${route.params.id}/editar`)
}

function goToBanking() {
  router.push(`/banking/persona/${route.params.id}`)
}

function confirmDelete() {
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!persona.value) return

  deleting.value = true
  try {
    await api.persona.delete(persona.value.id_Persona)
    uiStore.notifySuccess('Éxito', 'Persona eliminada correctamente')
    deleteModalOpen.value = false
    router.push('/personas')
  } catch (error: any) {
    console.error('Error deleting persona:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo eliminar la persona'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    deleting.value = false
  }
}

onMounted(() => {
  loadPersona()
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
          <h1 class="text-2xl font-bold text-gray-900">Detalle de Persona</h1>
          <p class="mt-1 text-sm text-gray-500">Información completa de la persona</p>
        </div>
      </div>
      <div v-if="!loading" class="flex space-x-3">
        <BaseButton variant="secondary" @click="goToBanking">
          <BanknotesIcon class="h-5 w-5 mr-2" />
          Cuentas Bancarias
        </BaseButton>
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
    <div v-else-if="persona" class="space-y-6">
      <!-- Personal Information -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información Personal</h2>
        </div>
        <div class="card-body">
          <dl class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
            <div>
              <dt class="text-sm font-medium text-gray-500">DNI / Cédula</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.dni }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Género</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ genderDisplay }}</dd>
            </div>
            <div class="md:col-span-2">
              <dt class="text-sm font-medium text-gray-500">Nombre Completo</dt>
              <dd class="mt-1 text-sm text-gray-900 font-medium">{{ nombreCompleto }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Primer Nombre</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.primerNombre }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Segundo Nombre</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.segundoNombre || '-' }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Apellido Paterno</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.apellidoPaterno }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Apellido Materno</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.apellidoMaterno }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Fecha de Nacimiento</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ fechaNacimientoDisplay }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Edad</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.edad }} años</dd>
            </div>
          </dl>
        </div>
      </div>

      <!-- Contact Information -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información de Contacto</h2>
        </div>
        <div class="card-body">
          <dl class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
            <div>
              <dt class="text-sm font-medium text-gray-500">Correos Electrónicos</dt>
              <dd class="mt-1 space-y-1">
                <div
                  v-for="(email, index) in persona.correo"
                  :key="index"
                  class="text-sm text-gray-900"
                >
                  {{ email }}
                </div>
                <div v-if="!persona.correo || persona.correo.length === 0" class="text-sm text-gray-400">
                  No registrado
                </div>
              </dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Teléfonos</dt>
              <dd class="mt-1 space-y-1">
                <div
                  v-for="(phone, index) in persona.telefono"
                  :key="index"
                  class="text-sm text-gray-900"
                >
                  {{ phone }}
                </div>
                <div v-if="!persona.telefono || persona.telefono.length === 0" class="text-sm text-gray-400">
                  No registrado
                </div>
              </dd>
            </div>
          </dl>
        </div>
      </div>

      <!-- Address -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Dirección</h2>
        </div>
        <div class="card-body">
          <dl class="grid grid-cols-1 md:grid-cols-3 gap-x-6 gap-y-4">
            <div class="md:col-span-2">
              <dt class="text-sm font-medium text-gray-500">Calle</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.direccion?.calle || '-' }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Número</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.direccion?.numero || '-' }}</dd>
            </div>
            <div class="md:col-span-3">
              <dt class="text-sm font-medium text-gray-500">Piso / Departamento</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.direccion?.piso || '-' }}</dd>
            </div>
            <div class="md:col-span-3">
              <dt class="text-sm font-medium text-gray-500">Dirección Completa</dt>
              <dd class="mt-1 text-sm text-gray-900 font-medium">{{ direccionCompleta }}</dd>
            </div>
          </dl>
        </div>
      </div>

      <!-- Metadata -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información del Sistema</h2>
        </div>
        <div class="card-body">
          <dl class="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
            <div>
              <dt class="text-sm font-medium text-gray-500">ID de Persona</dt>
              <dd class="mt-1 text-sm text-gray-900 font-mono">{{ persona.id_Persona }}</dd>
            </div>
            <div>
              <dt class="text-sm font-medium text-gray-500">Fecha de Creación</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ persona.dateCreated || '-' }}</dd>
            </div>
          </dl>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <BaseModal :open="deleteModalOpen" title="Confirmar Eliminación" @close="deleteModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea eliminar a <strong>{{ nombreCompleto }}</strong>? Esta acción no se
        puede deshacer.
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


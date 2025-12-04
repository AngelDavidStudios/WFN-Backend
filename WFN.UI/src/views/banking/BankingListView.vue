<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { PlusIcon, PencilIcon, TrashIcon, EyeIcon, ArrowLeftIcon } from '@heroicons/vue/24/outline'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { Banking, Persona } from '@/types'
import DataTable from '@/components/tables/DataTable.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const personaId = computed(() => route.params.personaId as string)

const loading = ref(true)
const loadingPersona = ref(true)
const cuentas = ref<Banking[]>([])
const persona = ref<Persona | null>(null)
const deleteModalOpen = ref(false)
const cuentaToDelete = ref<Banking | null>(null)
const deleting = ref(false)

const columns = [
  { key: 'bankName', label: 'Banco', width: '200px' },
  { key: 'accountTypeDisplay', label: 'Tipo de Cuenta', width: '150px' },
  { key: 'accountNumber', label: 'Número de Cuenta', width: '180px' },
  { key: 'pais', label: 'País', width: '120px' },
  { key: 'sucursal', label: 'Sucursal' },
]

const nombrePersona = computed(() => {
  if (!persona.value) return ''
  return [persona.value.primerNombre, persona.value.segundoNombre, persona.value.apellidoPaterno]
    .filter(Boolean)
    .join(' ')
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

async function loadCuentas() {
  loading.value = true
  try {
    const data = await api.banking.getByPersona(personaId.value)
    cuentas.value = data.map((c) => ({
      ...c,
      accountTypeDisplay: c.accountType === 'AHORROS' ? 'Ahorros' : 'Corriente',
    }))
  } catch (error: any) {
    console.error('Error loading cuentas:', error)
    // Si es 404, la persona no tiene cuentas, no es un error
    if (error.response?.status !== 404) {
      const errorMessage = error.response?.data?.message || 'No se pudieron cargar las cuentas bancarias'
      uiStore.notifyError('Error', errorMessage)
    }
    cuentas.value = []
  } finally {
    loading.value = false
  }
}

function goBack() {
  router.push('/personas')
}

function goToNew() {
  router.push(`/banking/persona/${personaId.value}/nuevo`)
}

function goToEdit(cuenta: Banking) {
  router.push(`/banking/persona/${personaId.value}/${cuenta.id_Banking}/editar`)
}

function goToView(cuenta: Banking) {
  router.push(`/banking/persona/${personaId.value}/${cuenta.id_Banking}`)
}

function confirmDelete(cuenta: Banking) {
  cuentaToDelete.value = cuenta
  deleteModalOpen.value = true
}

async function handleDelete() {
  if (!cuentaToDelete.value) return

  deleting.value = true
  try {
    await api.banking.delete(personaId.value, cuentaToDelete.value.id_Banking)
    uiStore.notifySuccess('Éxito', 'Cuenta bancaria eliminada correctamente')
    deleteModalOpen.value = false
    await loadCuentas()
  } catch (error: any) {
    console.error('Error deleting cuenta:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo eliminar la cuenta bancaria'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    deleting.value = false
    cuentaToDelete.value = null
  }
}

function handleRowClick(row: Record<string, unknown>) {
  const cuenta = row as unknown as Banking
  goToView(cuenta)
}

onMounted(async () => {
  await loadPersona()
  await loadCuentas()
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
          <h1 class="text-2xl font-bold text-gray-900">Cuentas Bancarias</h1>
          <p v-if="loadingPersona" class="mt-1 text-sm text-gray-500">Cargando...</p>
          <p v-else class="mt-1 text-sm text-gray-500">
            Cuentas de <strong>{{ nombrePersona }}</strong>
          </p>
        </div>
      </div>
      <BaseButton variant="primary" @click="goToNew" :disabled="loadingPersona">
        <PlusIcon class="h-5 w-5 mr-2" />
        Nueva Cuenta
      </BaseButton>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Table -->
    <DataTable
      v-else
      :columns="columns"
      :data="cuentas as unknown as Record<string, unknown>[]"
      :loading="loading"
      empty-text="No hay cuentas bancarias registradas"
      @row-click="handleRowClick"
    >
      <template #actions="{ row }">
        <div class="flex items-center space-x-2">
          <button
            type="button"
            class="text-gray-400 hover:text-blue-600"
            title="Ver"
            @click.stop="goToView(row as unknown as Banking)"
          >
            <EyeIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-blue-600"
            title="Editar"
            @click.stop="goToEdit(row as unknown as Banking)"
          >
            <PencilIcon class="h-5 w-5" />
          </button>
          <button
            type="button"
            class="text-gray-400 hover:text-red-600"
            title="Eliminar"
            @click.stop="confirmDelete(row as unknown as Banking)"
          >
            <TrashIcon class="h-5 w-5" />
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Delete Confirmation Modal -->
    <BaseModal :open="deleteModalOpen" title="Confirmar Eliminación" @close="deleteModalOpen = false">
      <p class="text-gray-500">
        ¿Está seguro que desea eliminar la cuenta bancaria de
        <strong>{{ cuentaToDelete?.bankName }}</strong> terminada en
        <strong>{{ cuentaToDelete?.accountNumber?.slice(-4) }}</strong>? Esta acción no se puede
        deshacer.
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


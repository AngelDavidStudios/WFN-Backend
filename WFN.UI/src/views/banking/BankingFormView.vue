<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { BankingCreateDTO, Persona, AccountType } from '@/types'
import { ACCOUNT_TYPE_OPTIONS, BANCOS_ECUADOR } from '@/constants'
import FormInput from '@/components/forms/FormInput.vue'
import FormSelect from '@/components/forms/FormSelect.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const personaId = computed(() => route.params.personaId as string)
const bankId = computed(() => route.params.bankId as string)
const isEdit = computed(() => !!bankId.value)
const pageTitle = computed(() => (isEdit.value ? 'Editar Cuenta Bancaria' : 'Nueva Cuenta Bancaria'))

const loading = ref(false)
const loadingPersona = ref(true)
const saving = ref(false)
const errors = ref<Record<string, string>>({})
const persona = ref<Persona | null>(null)

const form = ref<BankingCreateDTO>({
  bankName: '',
  accountNumber: '',
  accountType: 'AHORROS' as AccountType,
  swiftCode: '',
  pais: 'Ecuador',
  sucursal: '',
})

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

async function loadCuenta() {
  if (!isEdit.value) return

  loading.value = true
  try {
    const cuenta = await api.banking.getById(personaId.value, bankId.value)
    form.value = {
      bankName: cuenta.bankName,
      accountNumber: cuenta.accountNumber,
      accountType: cuenta.accountType,
      swiftCode: cuenta.swiftCode || '',
      pais: cuenta.pais,
      sucursal: cuenta.sucursal || '',
    }
  } catch (error: any) {
    console.error('Error loading cuenta:', error)
    const errorMessage = error.response?.data?.message || 'No se pudo cargar la cuenta bancaria'
    uiStore.notifyError('Error', errorMessage)
    router.push(`/banking/persona/${personaId.value}`)
  } finally {
    loading.value = false
  }
}

function validate(): boolean {
  errors.value = {}

  // Validación de banco
  if (!form.value.bankName) {
    errors.value.bankName = 'El banco es requerido'
  }

  // Validación de número de cuenta
  if (!form.value.accountNumber) {
    errors.value.accountNumber = 'El número de cuenta es requerido'
  } else if (form.value.accountNumber.length < 10) {
    errors.value.accountNumber = 'El número de cuenta debe tener al menos 10 dígitos'
  }

  // Validación de tipo de cuenta
  if (!form.value.accountType) {
    errors.value.accountType = 'El tipo de cuenta es requerido'
  }

  // Validación de país
  if (!form.value.pais) {
    errors.value.pais = 'El país es requerido'
  }

  return Object.keys(errors.value).length === 0
}

async function handleSubmit() {
  if (!validate()) return

  saving.value = true
  try {
    // Preparar datos según el backend
    const data: any = {
      bankName: form.value.bankName.trim(),
      accountNumber: form.value.accountNumber.trim(),
      accountType: form.value.accountType,
      swiftCode: form.value.swiftCode?.trim() || '',
      pais: form.value.pais.trim(),
      sucursal: form.value.sucursal?.trim() || '',
    }

    if (isEdit.value) {
      data.id_Banking = bankId.value
      await api.banking.update(personaId.value, bankId.value, data)
      uiStore.notifySuccess('Éxito', 'Cuenta bancaria actualizada correctamente')
    } else {
      await api.banking.create(personaId.value, data)
      uiStore.notifySuccess('Éxito', 'Cuenta bancaria creada correctamente')
    }
    router.push(`/banking/persona/${personaId.value}`)
  } catch (error: any) {
    console.error('Error saving cuenta:', error)
    const errorMessage = error.response?.data?.message || error.message || 'No se pudo guardar la cuenta bancaria'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push(`/banking/persona/${personaId.value}`)
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
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ pageTitle }}</h1>
        <p v-if="loadingPersona" class="mt-1 text-sm text-gray-500">Cargando...</p>
        <p v-else class="mt-1 text-sm text-gray-500">
          {{ isEdit ? 'Modifique los datos de la cuenta' : 'Complete los datos de la nueva cuenta' }} de
          <strong>{{ nombrePersona }}</strong>
        </p>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Form -->
    <form v-else class="space-y-6" @submit.prevent="handleSubmit">
      <!-- Información de la Cuenta -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información de la Cuenta</h2>
        </div>
        <div class="card-body">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormSelect
              v-model="form.bankName"
              label="Banco"
              :options="BANCOS_ECUADOR"
              required
              :error="errors.bankName"
            />
            <FormSelect
              v-model="form.accountType"
              label="Tipo de Cuenta"
              :options="ACCOUNT_TYPE_OPTIONS"
              required
              :error="errors.accountType"
            />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <FormInput
              v-model="form.accountNumber"
              label="Número de Cuenta"
              placeholder="1234567890"
              required
              minlength="10"
              maxlength="20"
              :error="errors.accountNumber"
              help-text="Mínimo 10 dígitos"
            />
            <FormInput
              v-model="form.swiftCode"
              label="Código SWIFT (Opcional)"
              placeholder="BPICEV22"
              maxlength="11"
              help-text="Para transferencias internacionales"
            />
          </div>
        </div>
      </div>

      <!-- Ubicación -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Ubicación</h2>
        </div>
        <div class="card-body">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormInput
              v-model="form.pais"
              label="País"
              placeholder="Ecuador"
              required
              :error="errors.pais"
            />
            <FormInput
              v-model="form.sucursal"
              label="Sucursal (Opcional)"
              placeholder="Matriz Quito"
            />
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex justify-end space-x-3">
        <BaseButton variant="outline" @click="goBack"> Cancelar </BaseButton>
        <BaseButton type="submit" variant="primary" :loading="saving">
          {{ isEdit ? 'Actualizar' : 'Crear' }} Cuenta
        </BaseButton>
      </div>
    </form>
  </div>
</template>


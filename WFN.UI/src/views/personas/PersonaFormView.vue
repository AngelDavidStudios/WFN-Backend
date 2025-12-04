<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { PersonaCreateDTO } from '@/types'
import { GENDER_OPTIONS } from '@/constants'
import FormInput from '@/components/forms/FormInput.vue'
import FormSelect from '@/components/forms/FormSelect.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const isEdit = computed(() => !!route.params.id)
const pageTitle = computed(() => (isEdit.value ? 'Editar Persona' : 'Nueva Persona'))

const loading = ref(false)
const saving = ref(false)
const errors = ref<Record<string, string>>({})

const form = ref<PersonaCreateDTO>({
  dni: '',
  gender: '',
  primerNombre: '',
  segundoNombre: '',
  apellidoPaterno: '',
  apellidoMaterno: '',
  dateBirthday: '',
  correo: [''],
  telefono: [''],
  direccion: {
    calle: '',
    numero: '',
    piso: '',
  },
})

async function loadPersona() {
  if (!isEdit.value) return

  loading.value = true
  try {
    const persona = await api.persona.getById(route.params.id as string)
    form.value = {
      dni: persona.dni,
      gender: persona.gender,
      primerNombre: persona.primerNombre,
      segundoNombre: persona.segundoNombre || '',
      apellidoPaterno: persona.apellidoPaterno,
      apellidoMaterno: persona.apellidoMaterno,
      dateBirthday: persona.dateBirthday?.split('T')[0] || '',
      correo: persona.correo?.length ? persona.correo : [''],
      telefono: persona.telefono?.length ? persona.telefono : [''],
      direccion: persona.direccion || { calle: '', numero: '', piso: '' },
    }
  } catch (error) {
    console.error('Error loading persona:', error)
    uiStore.notifyError('Error', 'No se pudo cargar la persona')
    router.push('/personas')
  } finally {
    loading.value = false
  }
}

function validate(): boolean {
  errors.value = {}

  // DNI validation
  if (!form.value.dni) {
    errors.value.dni = 'El DNI es requerido'
  } else if (form.value.dni.length < 10) {
    errors.value.dni = 'El DNI debe tener al menos 10 caracteres'
  }

  // Nombres validation
  if (!form.value.primerNombre) {
    errors.value.primerNombre = 'El primer nombre es requerido'
  }
  if (!form.value.apellidoPaterno) {
    errors.value.apellidoPaterno = 'El apellido paterno es requerido'
  }
  if (!form.value.apellidoMaterno) {
    errors.value.apellidoMaterno = 'El apellido materno es requerido'
  }

  // Gender validation
  if (!form.value.gender) {
    errors.value.gender = 'El género es requerido'
  }

  // Date validation
  if (!form.value.dateBirthday) {
    errors.value.dateBirthday = 'La fecha de nacimiento es requerida'
  } else {
    const birthday = new Date(form.value.dateBirthday)
    const today = new Date()
    if (birthday >= today) {
      errors.value.dateBirthday = 'La fecha de nacimiento debe ser anterior a hoy'
    }
  }

  // Email validation
  const nonEmptyEmails = form.value.correo.filter((e) => e.trim())
  for (const email of nonEmptyEmails) {
    if (!email.includes('@') || !email.includes('.')) {
      errors.value.correo = 'Uno o más correos no son válidos'
      break
    }
  }

  return Object.keys(errors.value).length === 0
}

async function handleSubmit() {
  if (!validate()) return

  saving.value = true
  try {
    // Prepare data according to backend model
    const data: any = {
      dni: form.value.dni.trim(),
      gender: form.value.gender,
      primerNombre: form.value.primerNombre.trim(),
      segundoNombre: form.value.segundoNombre?.trim() || '',
      apellidoPaterno: form.value.apellidoPaterno.trim(),
      apellidoMaterno: form.value.apellidoMaterno.trim(),
      dateBirthday: form.value.dateBirthday,
      correo: form.value.correo.filter((c) => c.trim()).map((c) => c.trim()),
      telefono: form.value.telefono.filter((t) => t.trim()).map((t) => t.trim()),
      direccion: {
        calle: form.value.direccion.calle?.trim() || '',
        numero: form.value.direccion.numero?.trim() || '',
        piso: form.value.direccion.piso?.trim() || '',
      },
    }

    if (isEdit.value) {
      data.id_Persona = route.params.id as string
      await api.persona.update(route.params.id as string, data)
      uiStore.notifySuccess('Éxito', 'Persona actualizada correctamente')
    } else {
      await api.persona.create(data)
      uiStore.notifySuccess('Éxito', 'Persona creada correctamente')
    }
    router.push('/personas')
  } catch (error: any) {
    console.error('Error saving persona:', error)
    const errorMessage = error.response?.data?.message || error.message || 'No se pudo guardar la persona'
    uiStore.notifyError('Error', errorMessage)
  } finally {
    saving.value = false
  }
}

function addEmail() {
  form.value.correo.push('')
}

function removeEmail(index: number) {
  if (form.value.correo.length > 1) {
    form.value.correo.splice(index, 1)
  }
}

function addPhone() {
  form.value.telefono.push('')
}

function removePhone(index: number) {
  if (form.value.telefono.length > 1) {
    form.value.telefono.splice(index, 1)
  }
}

function goBack() {
  router.push('/personas')
}

onMounted(() => {
  loadPersona()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ pageTitle }}</h1>
        <p class="mt-1 text-sm text-gray-500">
          {{ isEdit ? 'Modifique los datos de la persona' : 'Complete los datos de la nueva persona' }}
        </p>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Form -->
    <form v-else class="space-y-6" @submit.prevent="handleSubmit">
      <!-- Personal Information -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información Personal</h2>
        </div>
        <div class="card-body">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <FormInput
              v-model="form.dni"
              label="DNI / Cédula"
              placeholder="0123456789"
              required
              minlength="10"
              maxlength="13"
              :error="errors.dni"
              help-text="Mínimo 10 caracteres"
            />
            <FormSelect
              v-model="form.gender"
              label="Género"
              :options="GENDER_OPTIONS"
              required
              :error="errors.gender"
            />
            <FormInput
              v-model="form.dateBirthday"
              type="date"
              label="Fecha de Nacimiento"
              required
              :max="new Date().toISOString().split('T')[0]"
              :error="errors.dateBirthday"
            />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <FormInput
              v-model="form.primerNombre"
              label="Primer Nombre"
              placeholder="Juan"
              required
              :error="errors.primerNombre"
            />
            <FormInput
              v-model="form.segundoNombre"
              label="Segundo Nombre"
              placeholder="Carlos"
            />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <FormInput
              v-model="form.apellidoPaterno"
              label="Apellido Paterno"
              placeholder="Pérez"
              required
              :error="errors.apellidoPaterno"
            />
            <FormInput
              v-model="form.apellidoMaterno"
              label="Apellido Materno"
              placeholder="García"
              required
              :error="errors.apellidoMaterno"
            />
          </div>
        </div>
      </div>

      <!-- Contact Information -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información de Contacto</h2>
        </div>
        <div class="card-body">
          <!-- Emails -->
          <div class="space-y-2">
            <label class="label">Correos Electrónicos</label>
            <div v-for="(email, index) in form.correo" :key="index" class="flex gap-2">
              <input
                v-model="form.correo[index]"
                type="email"
                class="input flex-1"
                :class="{ 'border-red-500': errors.correo }"
                placeholder="correo@ejemplo.com"
              />
              <button
                type="button"
                class="btn-secondary btn-sm"
                @click="removeEmail(index)"
                :disabled="form.correo.length <= 1"
              >
                -
              </button>
            </div>
            <p v-if="errors.correo" class="text-sm text-red-600">{{ errors.correo }}</p>
            <button type="button" class="btn-secondary btn-sm" @click="addEmail">
              + Agregar correo
            </button>
          </div>

          <!-- Phones -->
          <div class="space-y-2 mt-4">
            <label class="label">Teléfonos</label>
            <div v-for="(phone, index) in form.telefono" :key="index" class="flex gap-2">
              <input
                v-model="form.telefono[index]"
                type="tel"
                class="input flex-1"
                placeholder="0999999999"
                maxlength="10"
              />
              <button
                type="button"
                class="btn-secondary btn-sm"
                @click="removePhone(index)"
                :disabled="form.telefono.length <= 1"
              >
                -
              </button>
            </div>
            <button type="button" class="btn-secondary btn-sm" @click="addPhone">
              + Agregar teléfono
            </button>
          </div>
        </div>
      </div>

      <!-- Address -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Dirección</h2>
        </div>
        <div class="card-body">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <FormInput
              v-model="form.direccion.calle"
              label="Calle"
              placeholder="Av. Principal"
              class="md:col-span-2"
            />
            <FormInput
              v-model="form.direccion.numero"
              label="Número"
              placeholder="123"
            />
            <FormInput
              v-model="form.direccion.piso"
              label="Piso / Departamento"
              placeholder="2do piso"
            />
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex justify-end space-x-3">
        <BaseButton variant="outline" @click="goBack"> Cancelar </BaseButton>
        <BaseButton type="submit" variant="primary" :loading="saving">
          {{ isEdit ? 'Actualizar' : 'Crear' }} Persona
        </BaseButton>
      </div>
    </form>
  </div>
</template>
